using System.Windows;
using ReadToUnlock.Services;

namespace ReadToUnlock;

public partial class App : Application
{
    public static ConfigService ConfigService { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ConfigService = new ConfigService();
    }
}