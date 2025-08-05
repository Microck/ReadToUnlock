using System.Windows;

namespace ReadToUnlock;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void SettingsMenu_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow
        {
            Owner = this
        };
        settingsWindow.ShowDialog();
    }

    private void ExitMenu_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void StartReading_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Reading session started!", "ReadToUnlock", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}