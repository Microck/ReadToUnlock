using System.Windows;
using System.IO;
using Microsoft.Win32;
using ReadToUnlock.Models;
using ReadToUnlock.Services;

namespace ReadToUnlock;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var config = App.ConfigService.Config;
        
        MinWordsText.Text = config.MinWords.ToString();
        MaxWordsText.Text = config.MaxWords.ToString();
        EnglishRequiredText.Text = config.EnglishRequired.ToString();
        SpanishRequiredText.Text = config.SpanishRequired.ToString();
        SingleLanguageModeCheck.IsChecked = config.SingleLanguageMode;
        AccuracyThresholdText.Text = config.AccuracyThreshold.ToString();
        EnglishQuotesPathText.Text = config.EnglishQuotesPath;
        SpanishQuotesPathText.Text = config.SpanishQuotesPath;
        EmergencyPasswordBox.Password = config.EmergencyPassword;
        EmergencyHotkeyText.Text = config.EmergencyHotkey;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInputs())
            return;

        var config = App.ConfigService.Config;
        
        config.MinWords = int.Parse(MinWordsText.Text);
        config.MaxWords = int.Parse(MaxWordsText.Text);
        config.EnglishRequired = int.Parse(EnglishRequiredText.Text);
        config.SpanishRequired = int.Parse(SpanishRequiredText.Text);
        config.SingleLanguageMode = SingleLanguageModeCheck.IsChecked ?? false;
        config.AccuracyThreshold = int.Parse(AccuracyThresholdText.Text);
        config.EnglishQuotesPath = EnglishQuotesPathText.Text;
        config.SpanishQuotesPath = SpanishQuotesPathText.Text;
        config.EmergencyPassword = EmergencyPasswordBox.Password;
        config.EmergencyHotkey = EmergencyHotkeyText.Text;

        App.ConfigService.SaveConfig();
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        App.ConfigService.ResetToDefaults();
        LoadSettings();
    }

    private void ValidateButton_Click(object sender, RoutedEventArgs e)
    {
        var config = App.ConfigService.Config;
        
        var englishPath = QuoteService.ResolvePath(config.EnglishQuotesPath);
        var spanishPath = QuoteService.ResolvePath(config.SpanishQuotesPath);
        
        var englishExists = File.Exists(englishPath);
        var spanishExists = File.Exists(spanishPath);
        
        var message = $"File Validation Results:\n\n" +
                     $"English quotes: {(englishExists ? "✓ Found" : "✗ Not found")}\n" +
                     $"  Path: {englishPath}\n\n" +
                     $"Spanish quotes: {(spanishExists ? "✓ Found" : "✗ Not found")}\n" +
                     $"  Path: {spanishPath}\n\n" +
                     $"If files are not found, please use the Browse buttons to select the correct files.";
        
        MessageBox.Show(message, "File Validation", MessageBoxButton.OK, 
                       englishExists && spanishExists ? MessageBoxImage.Information : MessageBoxImage.Warning);
    }

    private void BrowseEnglishButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Select English Quotes File",
            DefaultExt = ".json"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            EnglishQuotesPathText.Text = openFileDialog.FileName;
        }
    }

    private void BrowseSpanishButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Select Spanish Quotes File",
            DefaultExt = ".json"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            SpanishQuotesPathText.Text = openFileDialog.FileName;
        }
    }

    private bool ValidateInputs()
    {
        if (!int.TryParse(MinWordsText.Text, out var minWords) || minWords <= 0)
        {
            MessageBox.Show("Please enter a valid positive number for minimum words.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!int.TryParse(MaxWordsText.Text, out var maxWords) || maxWords <= 0)
        {
            MessageBox.Show("Please enter a valid positive number for maximum words.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (minWords > maxWords)
        {
            MessageBox.Show("Minimum words must be less than or equal to maximum words.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!int.TryParse(EnglishRequiredText.Text, out var englishReq) || englishReq < 0)
        {
            MessageBox.Show("Please enter a valid non-negative number for English passages required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!int.TryParse(SpanishRequiredText.Text, out var spanishReq) || spanishReq < 0)
        {
            MessageBox.Show("Please enter a valid non-negative number for Spanish passages required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!int.TryParse(AccuracyThresholdText.Text, out var accuracy) || accuracy < 1 || accuracy > 100)
        {
            MessageBox.Show("Please enter a valid accuracy threshold between 1 and 100.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }



        return true;
    }
}