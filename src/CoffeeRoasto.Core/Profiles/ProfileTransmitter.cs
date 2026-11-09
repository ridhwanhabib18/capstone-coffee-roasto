using System.Data;
using System.Diagnostics;
using CoffeeRoasto.Hardware;

namespace CoffeeRoasto.Profiles;

/// <summary>One line to send to the firmware.</summary>
/// <param name="Text">The comma-separated set points.</param>
/// <param name="IsFullGroup">True for a group of ten values; the sender then waits for the firmware to use them.</param>
public readonly record struct ProfileChunk(string Text, bool IsFullGroup);

/// <summary>
/// Sends a profile to the firmware in groups of ten values, 40 seconds apart
/// (ten set points last about 40 s: one every ~4 s). Runs on a background thread.
/// </summary>
public sealed class ProfileTransmitter
{
    public const int GroupSize = 10;
    public static readonly TimeSpan GroupDelay = TimeSpan.FromSeconds(40);

    private readonly RoasterLink _link;
    private int _running;

    public ProfileTransmitter(RoasterLink link)
    {
        _link = link;
    }

    public bool IsRunning => _running == 1;

    /// <summary>Raised when a transfer ends, with null on success or the error message.</summary>
    public event Action<string?>? Finished;

    /// <summary>
    /// Splits the values into lines: nine values with a comma, a tenth without one.
    /// A shorter last group keeps its trailing comma, which the firmware tolerates.
    /// </summary>
    public static IEnumerable<ProfileChunk> BuildChunks(IReadOnlyList<string> values)
    {
        string payload = "";
        for (int i = 0; i < values.Count; i++)
        {
            if ((i + 1) % GroupSize == 0)
            {
                yield return new ProfileChunk(payload + values[i], true);
                payload = "";
            }
            else
            {
                payload += values[i] + ",";
            }
        }

        if (payload != "")
        {
            yield return new ProfileChunk(payload, false);
        }
    }

    /// <summary>Starts sending. Throws InvalidOperationException when a transfer is already running.</summary>
    public void Start(DataTable table)
    {
        var values = new List<string>(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            values.Add(Convert.ToString(row[ProfileReader.BeanTemperatureColumn], System.Globalization.CultureInfo.InvariantCulture) ?? "");
        }

        if (Interlocked.Exchange(ref _running, 1) == 1)
        {
            throw new InvalidOperationException("A profile is already being sent.");
        }

        _ = Task.Run(async () =>
        {
            string? error = null;
            try
            {
                foreach (ProfileChunk chunk in BuildChunks(values))
                {
                    _link.SendLine(chunk.Text);
                    if (chunk.IsFullGroup)
                    {
                        await Task.Delay(GroupDelay);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                Interlocked.Exchange(ref _running, 0);
                Debug.WriteLine(error != null ? "profile transfer failed: " + error : "profile transfer finished");
                Finished?.Invoke(error);
            }
        });
    }
}
