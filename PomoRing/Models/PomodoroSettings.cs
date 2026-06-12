namespace PomoRing.Models;

public sealed class PomodoroSettings
{
    public string CharacterType { get; set; } = "Cat";
    public bool CountUpEnabled { get; set; }
    public int FocusMinutes { get; set; } = 25;
    public int RestMinutes { get; set; } = 5;
    public bool RepeatEnabled { get; set; } = true;
    public int RepeatCount { get; set; } = 4;
    public bool ShowTimer { get; set; } = true;
}
