using System.IO;
using System.Text;
using System.Windows;
using ReadToUnlock.Services;

namespace ReadToUnlock;

public partial class DebugWindow : Window
{
    public DebugWindow()
    {
        InitializeComponent();
        GenerateDebugInfo();
    }

    private void GenerateDebugInfo()
    {
        var sb = new StringBuilder();
        var config = App.ConfigService.Config;
        
        sb.AppendLine("=== READTOUNLOCK DEBUG INFORMATION ===");
        sb.AppendLine();
        
        // Configuration
        sb.AppendLine("=== CONFIGURATION ===");
        sb.AppendLine($"English Quotes Path: {config.EnglishQuotesPath}");
        sb.AppendLine($"Spanish Quotes Path: {config.SpanishQuotesPath}");
        sb.AppendLine();
        
        // Path resolution
        sb.AppendLine("=== PATH RESOLUTION ===");
        var englishResolved = QuoteService.ResolvePath(config.EnglishQuotesPath);
        var spanishResolved = QuoteService.ResolvePath(config.SpanishQuotesPath);
        
        sb.AppendLine($"English resolved: {englishResolved}");
        sb.AppendLine($"English exists: {File.Exists(englishResolved)}");
        sb.AppendLine($"Spanish resolved: {spanishResolved}");
        sb.AppendLine($"Spanish exists: {File.Exists(spanishResolved)}");
        sb.AppendLine();
        
        // Working directory info
        sb.AppendLine("=== WORKING DIRECTORY INFO ===");
        sb.AppendLine($"Current Directory: {Directory.GetCurrentDirectory()}");
        sb.AppendLine($"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
        sb.AppendLine($"Executable Path: {System.Reflection.Assembly.GetExecutingAssembly().Location}");
        sb.AppendLine();
        
        // Test actual loading
        sb.AppendLine("=== QUOTE LOADING TEST ===");
        var quoteService = new QuoteService(App.ConfigService);
        var englishCount = quoteService.GetRandomEnglishQuote() != null ? 1 : 0;
        var spanishCount = quoteService.GetRandomSpanishQuote() != null ? 1 : 0;
        
        sb.AppendLine($"English quotes loaded: {englishCount}");
        sb.AppendLine($"Spanish quotes loaded: {spanishCount}");
        sb.AppendLine();
        
        // File details
        sb.AppendLine("=== FILE DETAILS ===");
        
        if (File.Exists(englishResolved))
        {
            try
            {
                var content = File.ReadAllText(englishResolved);
                sb.AppendLine($"English file size: {content.Length} bytes");
                sb.AppendLine($"English file content:");
                sb.AppendLine(content);
                sb.AppendLine();
                
                // Test JSON parsing
                try
                {
                    var collection = System.Text.Json.JsonSerializer.Deserialize<QuoteCollection>(content);
                    sb.AppendLine($"JSON parsing test: {collection?.Quotes?.Count ?? 0} quotes found");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"JSON parsing error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error reading English file: {ex.Message}");
            }
        }
        
        if (File.Exists(spanishResolved))
        {
            try
            {
                var content = File.ReadAllText(spanishResolved);
                sb.AppendLine($"Spanish file size: {content.Length} bytes");
                sb.AppendLine($"Spanish file content:");
                sb.AppendLine(content);
                sb.AppendLine();
                
                // Test JSON parsing
                try
                {
                    var collection = System.Text.Json.JsonSerializer.Deserialize<QuoteCollection>(content);
                    sb.AppendLine($"JSON parsing test: {collection?.Quotes?.Count ?? 0} quotes found");
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"JSON parsing error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error reading Spanish file: {ex.Message}");
            }
        }
        
        DebugText.Text = sb.ToString();
    }

    private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(DebugText.Text);
        MessageBox.Show("Debug information copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ReloadQuotes_Click(object sender, RoutedEventArgs e)
    {
        GenerateDebugInfo();
    }
}