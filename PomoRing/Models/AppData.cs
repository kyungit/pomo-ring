namespace PomoRing.Models;

public sealed class AppData
{
    public PomodoroSettings Settings { get; set; } = new();
    public UserProgress Progress { get; set; } = new();
}
