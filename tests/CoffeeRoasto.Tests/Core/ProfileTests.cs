using System.Data;
using CoffeeRoasto.Profiles;

namespace CoffeeRoasto.Tests.Core;

public class ProfileTests
{
    // The sendtohardware loop of the v1 app, reduced to the lines it wrote.
    private static List<string> LegacyChunks(List<string> values)
    {
        var lines = new List<string>();
        var payload = "";
        for (var i = 0; i < values.Count; i++)
        {
            if ((i + 1) % 10 == 0)
            {
                payload += values[i];
                lines.Add(payload);
                payload = "";
            }
            else
            {
                payload += values[i] + ",";
            }
        }
        if (payload != "") lines.Add(payload);
        return lines;
    }

    [Fact]
    public void Chunks_match_the_original_algorithm_for_0_to_45_rows()
    {
        for (int n = 0; n <= 45; n++)
        {
            var values = Enumerable.Range(1, n).Select(i => (100 + i).ToString()).ToList();
            var chunks = ProfileTransmitter.BuildChunks(values).Select(c => c.Text).ToList();

            Assert.Equal(LegacyChunks(values), chunks);
        }
    }

    [Fact]
    public void Only_full_groups_are_followed_by_the_wait()
    {
        var groups = ProfileTransmitter.BuildChunks(Enumerable.Range(1, 23).Select(i => i.ToString()).ToList()).ToList();

        Assert.Equal(3, groups.Count);
        Assert.True(groups[0].IsFullGroup);
        Assert.True(groups[1].IsFullGroup);
        Assert.False(groups[2].IsFullGroup);
        Assert.Equal("1,2,3,4,5,6,7,8,9,10", groups[0].Text);   // no trailing comma
        Assert.Equal("21,22,23,", groups[2].Text);              // the short group keeps it
        Assert.Equal(TimeSpan.FromSeconds(40), ProfileTransmitter.GroupDelay);
    }

    private static string ProfilePath(string file)
    {
        string? dir = AppContext.BaseDirectory;
        while (dir != null && !Directory.Exists(Path.Combine(dir, "profiles")))
        {
            dir = Path.GetDirectoryName(dir);
        }
        Assert.NotNull(dir);
        return Path.Combine(dir!, "profiles", file);
    }

    [Fact]
    public void The_sample_xlsx_profile_has_427_bean_temperatures()
    {
        DataTableCollection sheets = ProfileReader.Read(ProfilePath("Testing Baru.xls.xlsx"));

        DataTable roast = sheets["Exported from gridview"]!;
        Assert.Single(roast.Columns);
        Assert.Equal(ProfileReader.BeanTemperatureColumn, roast.Columns[0].ColumnName);
        Assert.Equal(427, roast.Rows.Count);
        Assert.Equal("149.91", Convert.ToString(roast.Rows[0][0], System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void The_old_binary_xls_profile_can_be_read_too()
    {
        DataTableCollection sheets = ProfileReader.Read(ProfilePath("Testing.xls"));

        Assert.True(sheets.Count > 0);
    }
}
