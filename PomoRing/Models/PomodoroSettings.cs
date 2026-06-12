namespace PomoRing.Models;

public sealed class PomodoroSettings
{
    public string CharacterType { get; set; } = "Cat";
    public bool CountUpEnabled { get; set; }
    public bool RestEnabled { get; set; } = true;
    public bool UseSecondsForTesting { get; set; }
    public int FocusMinutes { get; set; } = 50;
    public int RestMinutes { get; set; } = 10;
    public bool RepeatEnabled { get; set; } = true;
    public int RepeatCount { get; set; } = 4;
    public bool ShowTimer { get; set; } = true;
}
