namespace ReadToUnlock.Models;

public class Config
{
    // Passage Settings
    public int MinWords { get; set; } = 60;
    public int MaxWords { get; set; } = 100;

    // Language Settings
    public int EnglishRequired { get; set; } = 1;
    public int SpanishRequired { get; set; } = 1;
    public bool SingleLanguageMode { get; set; } = false;

    // Recognition Settings
    public int AccuracyThreshold { get; set; } = 90;
    public int MaxPauseTime { get; set; } = 3;

    // Emergency Settings
    public string EmergencyPassword { get; set; } = "unlock";
    public string EmergencyHotkey { get; set; } = "Ctrl+Shift+U";
}