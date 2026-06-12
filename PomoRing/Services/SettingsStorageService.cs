using System.IO;
using System.Text.Json;
using PomoRing.Models;

namespace PomoRing.Services;

public sealed class SettingsStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PomoRing",
        "settings.json");

    public AppData Load()
    {
        try
        {
            if (!File.Exists(_path))
                return new AppData();

            var data = JsonSerializer.Deserialize<AppData>(File.ReadAllText(_path), JsonOptions) ?? new AppData();
            if (data.Progress.LastStudyDate != DateOnly.FromDateTime(DateTime.Today))
            {
                data.Progress.LastStudyDate = DateOnly.FromDateTime(DateTime.Today);
                data.Progress.TodayFocusMinutes = 0;
            }
            return data;
        }
        catch
        {
            return new AppData();
        }
    }

    public void Save(AppData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(data, JsonOptions));
    }
}
