using System.Windows;
using System.Windows.Input;

namespace ReadToUnlock;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartReading_Click(object sender, RoutedEventArgs e)
    {
        StartReadingSession();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow
        {
            Owner = this
        };
        settingsWindow.ShowDialog();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Application.Current.Shutdown();
        }
        else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        {
            SettingsButton_Click(sender, e);
        }
    }

    private void StartReadingSession()
    {
        var readingWindow = new ReadingWindow();
        readingWindow.Show();
        this.Hide();
    }
}