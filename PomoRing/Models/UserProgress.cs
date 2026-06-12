namespace PomoRing.Models;

public sealed class UserProgress
{
    public int TotalFocusMinutes { get; set; }
    public DateOnly LastStudyDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int TodayFocusMinutes { get; set; }
}
