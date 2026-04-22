using EventHarbor.Domain;

namespace EventHarbor.Tests;

public class CultureActionStatusTests
{
    [Fact]
    public void Status_is_Planned_when_Start_in_future()
    {
        var a = new CultureAction
        {
            StartAt = DateTime.Now.AddDays(7),
            EndAt = DateTime.Now.AddDays(7).AddHours(3),
        };
        Assert.Equal(EventStatus.Planned, a.Status);
    }

    [Fact]
    public void Status_is_Running_when_now_between_Start_and_End()
    {
        var a = new CultureAction
        {
            StartAt = DateTime.Now.AddHours(-1),
            EndAt = DateTime.Now.AddHours(1),
        };
        Assert.Equal(EventStatus.Running, a.Status);
    }

    [Fact]
    public void Status_is_Ended_when_End_in_past()
    {
        var a = new CultureAction
        {
            StartAt = DateTime.Now.AddDays(-7),
            EndAt = DateTime.Now.AddDays(-7).AddHours(2),
        };
        Assert.Equal(EventStatus.Ended, a.Status);
    }

    [Fact]
    public void DisplayId_formats_from_CreatedAt_year_and_zero_padded_id()
    {
        var a = new CultureAction
        {
            Id = 42,
            CreatedAt = new DateTime(2026, 3, 15),
        };
        Assert.Equal("EV-2026-042", a.DisplayId);
    }

    [Fact]
    public void TotalAttendance_sums_four_categories()
    {
        var a = new CultureAction
        {
            Children = 10,
            Adults = 40,
            Seniors = 5,
            Disabled = 2,
        };
        Assert.Equal(57, a.TotalAttendance);
    }
}
