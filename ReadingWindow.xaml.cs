using System.Windows;
using System.Windows.Input;
using ReadToUnlock.Services;

namespace ReadToUnlock;

public partial class ReadingWindow : Window
{
    private readonly QuoteService _quoteService;
    private int _currentQuoteIndex = 0;

    public ReadingWindow()
    {
        InitializeComponent();
        _quoteService = new QuoteService();
        LoadNextPassage();
    }

    private void LoadNextPassage()
    {
        var quote = _quoteService.GetRandomQuote();
        if (quote != null)
        {
            PassageText.Text = quote.Text;
            _currentQuoteIndex++;
        }
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Reading started! Use Ctrl+S for settings or Escape to exit.", 
                       "Reading Practice", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        LoadNextPassage();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            BackButton_Click(sender, e);
        }
        else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
        }
    }
}