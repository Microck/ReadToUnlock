using System.Text.Json;
using System.IO;
using System;
using System.Linq;

namespace ReadToUnlock.Services;

public class Quote
{
    public string Text { get; set; } = "";
    public string Author { get; set; } = "";
}

public class QuoteCollection
{
    public List<Quote> Quotes { get; set; } = new();
}

public class QuoteService
{
    private List<Quote> _englishQuotes = new();
    private List<Quote> _spanishQuotes = new();
    private readonly Random _random = new();

    private readonly ConfigService _configService;

    public QuoteService(ConfigService configService)
    {
        _configService = configService;
        ReloadQuotes();
    }

    public void ReloadQuotes()
    {
        _englishQuotes = LoadQuotes(_configService.Config.EnglishQuotesPath);
        _spanishQuotes = LoadQuotes(_configService.Config.SpanishQuotesPath);
        
        Console.WriteLine($"Loaded {_englishQuotes.Count} English quotes from {_configService.Config.EnglishQuotesPath}");
        Console.WriteLine($"Loaded {_spanishQuotes.Count} Spanish quotes from {_configService.Config.SpanishQuotesPath}");
        
        // Add fallback quotes if files are empty
        if (_englishQuotes.Count == 0)
        {
            _englishQuotes = GetFallbackEnglishQuotes();
            Console.WriteLine("Using fallback English quotes");
        }
        
        if (_spanishQuotes.Count == 0)
        {
            _spanishQuotes = GetFallbackSpanishQuotes();
            Console.WriteLine("Using fallback Spanish quotes");
        }
    }

    private List<Quote> GetFallbackEnglishQuotes()
    {
        return new List<Quote>
        {
            new Quote { Text = "The quick brown fox jumps over the lazy dog. This is a simple sentence to practice reading.", Author = "Practice" },
            new Quote { Text = "Reading is fundamental to learning and growth. Practice makes perfect.", Author = "Education" },
            new Quote { Text = "Every great reader started with simple sentences and built their skills over time.", Author = "Motivation" }
        };
    }

    private List<Quote> GetFallbackSpanishQuotes()
    {
        return new List<Quote>
        {
            new Quote { Text = "El rápido zorro marrón salta sobre el perro perezoso. Esta es una oración simple para practicar la lectura.", Author = "Práctica" },
            new Quote { Text = "Leer es fundamental para el aprendizaje y el crecimiento. La práctica hace al maestro.", Author = "Educación" },
            new Quote { Text = "Todo gran lector comenzó con oraciones simples y desarrolló sus habilidades con el tiempo.", Author = "Motivación" }
        };
    }

    private List<Quote> LoadQuotes(string fileName)
    {
        try
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(baseDirectory, fileName);
            
            // Debug: Check all possible paths
            Console.WriteLine($"Looking for {fileName} at: {fullPath}");
            Console.WriteLine($"File exists: {File.Exists(fullPath)}");
            
            // Also check the project root
            var projectPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            Console.WriteLine($"Also checking project path: {projectPath} - exists: {File.Exists(projectPath)}");
            
            // Check if we're in bin/Debug or bin/Release and go up to project root
            var upPath = Path.Combine(baseDirectory, "..", "..", "..", fileName);
            Console.WriteLine($"Checking up path: {upPath} - exists: {File.Exists(upPath)}");
            
            string[] pathsToCheck = { fullPath, projectPath, upPath };
            
            foreach (var path in pathsToCheck)
            {
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    Console.WriteLine($"Successfully loaded {fileName} from {path}");
                    Console.WriteLine($"JSON content preview: {json.Substring(0, Math.Min(100, json.Length))}...");
                    
                    try
                    {
                        var collection = JsonSerializer.Deserialize<QuoteCollection>(json);
                        var quotes = collection?.Quotes ?? new List<Quote>();
                        Console.WriteLine($"Parsed {quotes.Count} quotes from {fileName}");
                        return quotes;
                    }
                    catch (JsonException jex)
                    {
                        Console.WriteLine($"JSON parsing error: {jex.Message}");
                    }
                }
            }
            
            Console.WriteLine($"Quote file not found in any location: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading {fileName}: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        return new List<Quote>();
    }

    public Quote? GetRandomQuote(string? language = null)
    {
        List<Quote> quotes;
        
        if (language?.ToLower() == "spanish")
            quotes = _spanishQuotes;
        else if (language?.ToLower() == "english")
            quotes = _englishQuotes;
        else
            quotes = _random.Next(2) == 0 ? _englishQuotes : _spanishQuotes;

        if (quotes.Count == 0)
            return null;

        var index = _random.Next(quotes.Count);
        return quotes[index];
    }

    public Quote? GetRandomEnglishQuote() => GetRandomQuote("english");
    public Quote? GetRandomSpanishQuote() => GetRandomQuote("spanish");
}