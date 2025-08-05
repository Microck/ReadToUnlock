using System.IO;
using System.Text.Json;
using ReadToUnlock.Models;

namespace ReadToUnlock.Services;

public class ConfigService
{
    private const string ConfigFileName = "config.json";
    public Config Config { get; private set; }

    public ConfigService()
    {
        Config = new Config();
        LoadConfig();
    }

    public void LoadConfig()
    {
        if (File.Exists(ConfigFileName))
        {
            var json = File.ReadAllText(ConfigFileName);
            Config = JsonSerializer.Deserialize<Config>(json) ?? new Config();
        }
        else
        {
            SaveConfig();
        }
    }

    public void SaveConfig()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(Config, options);
        File.WriteAllText(ConfigFileName, json);
    }

    public void ResetToDefaults()
    {
        Config = new Config();
        SaveConfig();
    }
}