using CoffeeRoasto.ViewModels;

namespace CoffeeRoasto.Tests.ViewModels;

/// <summary>The view model without a window and without a serial port (the port stays closed).</summary>
public class MainViewModelTests
{
    private static MainViewModel Create(out Func<double> clock, out List<Notification> notes)
    {
        double seconds = 0;
        clock = () => seconds;
        var list = new List<Notification>();
        notes = list;

        var vm = new MainViewModel
        {
            Post = action => action(),
            ElapsedProvider = () => TimeSpan.FromSeconds(seconds)
        };
        vm.NotificationRaised += list.Add;
        return vm;
    }

    [Fact]
    public void A_data_frame_updates_the_readings_and_the_chart()
    {
        MainViewModel vm = Create(out _, out _);

        vm.HandleLine("@157.56A182.38B158.39C");

        Assert.Equal("157.6", vm.BeanText);
        Assert.Equal("182.4", vm.DrumText);
        Assert.Equal("158.4", vm.ReferenceText);
        Assert.Equal("-0.8", vm.DeltaText);
        Assert.Single(vm.History.Bean);
        Assert.Equal(157.56, vm.History.Bean[0], 9);
    }

    [Fact]
    public void Without_a_reference_the_delta_is_not_shown()
    {
        MainViewModel vm = Create(out _, out _);

        vm.HandleLine("@120.00A130.00B0.00C");

        Assert.Equal("—", vm.ReferenceText);
        Assert.Equal("—", vm.DeltaText);
    }

    [Fact]
    public void Rate_of_rise_is_degrees_per_minute_over_the_last_minute()
    {
        double now = 0;
        var vm = new MainViewModel { Post = a => a(), ElapsedProvider = () => TimeSpan.FromSeconds(now) };

        now = 0; vm.HandleLine("@100.00A100.00B0.00C");
        Assert.Equal("—", vm.RateText);                       // not enough time yet

        now = 30; vm.HandleLine("@110.00A100.00B0.00C");
        Assert.Equal("+20.0", vm.RateText);                   // 10 °C in 30 s

        now = 90; vm.HandleLine("@130.00A100.00B0.00C");      // only the last minute counts: from 30 s to 90 s
        Assert.Equal("+20.0", vm.RateText);                   // 20 °C in 60 s
    }

    [Fact]
    public void The_events_set_the_burner_lamp_and_raise_toasts()
    {
        MainViewModel vm = Create(out _, out List<Notification> notes);

        vm.HandleLine("Y");
        Assert.True(vm.BurnerLamp);
        vm.HandleLine("Z");
        Assert.False(vm.BurnerLamp);

        vm.PidLamp = vm.BlowerLamp = true;
        vm.HandleLine("X");

        Assert.False(vm.PidLamp);
        Assert.False(vm.BlowerLamp);
        Assert.True(vm.DrumLamp);
        Assert.True(vm.CoolerLamp);
        Assert.Contains(notes, n => n.Title == "Roasting finished" && n.Level == NotificationLevel.Success);
    }

    [Fact]
    public void A_button_does_not_light_its_lamp_while_the_port_is_closed()
    {
        MainViewModel vm = Create(out _, out _);

        vm.SwitchCommand.Execute("BurnerOn");
        vm.StartPidCommand.Execute(null);

        Assert.False(vm.BurnerLamp);
        Assert.False(vm.PidLamp);
    }

    [Fact]
    public void An_unknown_actuator_name_is_a_programming_error()
    {
        MainViewModel vm = Create(out _, out _);

        Assert.Throws<ArgumentException>(() => vm.SwitchCommand.Execute("Toaster"));
    }

    [Fact]
    public void A_set_point_that_is_not_a_number_is_refused_with_a_message()
    {
        MainViewModel vm = Create(out _, out List<Notification> notes);

        vm.SetpointText = "abc";
        vm.SendSetpointCommand.Execute(null);

        Assert.Contains(notes, n => n.Level == NotificationLevel.Warning);
        Assert.Equal("abc", vm.SetpointText);
    }

    [Fact]
    public void Connecting_without_a_port_asks_for_one()
    {
        MainViewModel vm = Create(out _, out List<Notification> notes);
        vm.SelectedPort = null;

        vm.ConnectCommand.Execute(null);

        Assert.False(vm.IsConnected);
        Assert.Contains(notes, n => n.Title == "Connection" && n.Level == NotificationLevel.Warning);
    }

    [Fact]
    public void Recording_keeps_the_samples_and_exporting_nothing_is_refused()
    {
        MainViewModel vm = Create(out _, out List<Notification> notes);

        vm.ExportRecordingCommand.Execute(null);
        Assert.Contains(notes, n => n.Title == "Export" && n.Level == NotificationLevel.Warning);

        vm.StartRecordingCommand.Execute(null);
        vm.HandleLine("@100.00A110.00B100.00C");
        vm.HandleLine("@101.00A111.00B100.00C");
        vm.StopRecordingCommand.Execute(null);

        Assert.Equal("Recording · 2 samples", vm.TableTitle);
        Assert.Equal(2, ((System.Collections.ICollection)vm.TableSource!).Count);
        Assert.False(vm.RecordLamp);
    }
}
