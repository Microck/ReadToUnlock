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
        
        // File details
        sb.AppendLine("=== FILE DETAILS ===");
        
        if (File.Exists(englishResolved))
        {
            try
            {
                var content = File.ReadAllText(englishResolved);
                sb.AppendLine($"English file size: {content.Length} bytes");
                sb.AppendLine($"English file preview: {content.Substring(0, Math.Min(200, content.Length))}");
                sb.AppendLine();
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
                sb.AppendLine($"Spanish file preview: {content.Substring(0, Math.Min(200, content.Length))}");
                sb.AppendLine();
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error reading Spanish file: {ex.Message}");
            }
        }
        
        // Directory listing
        sb.AppendLine("=== DIRECTORY CONTENTS ===");
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(baseDir, "*.json");
            sb.AppendLine($"JSON files in base directory:");
            foreach (var file in files)
            {
                sb.AppendLine($"  {Path.GetFileName(file)}");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"Error listing directory: {ex.Message}");
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
}