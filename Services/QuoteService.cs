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
        Console.WriteLine("=== Reloading Quotes ===");
        
        var englishPath = _configService.Config.EnglishQuotesPath;
        var spanishPath = _configService.Config.SpanishQuotesPath;
        
        Console.WriteLine($"Configured English path: {englishPath}");
        Console.WriteLine($"Configured Spanish path: {spanishPath}");
        
        _englishQuotes = LoadQuotes(englishPath);
        _spanishQuotes = LoadQuotes(spanishPath);
        
        Console.WriteLine($"Final count - English: {_englishQuotes.Count}, Spanish: {_spanishQuotes.Count}");
        
        // Only use fallback if truly no quotes loaded
        if (_englishQuotes.Count == 0 && !File.Exists(ResolvePath(englishPath)))
        {
            _englishQuotes = GetFallbackEnglishQuotes();
            Console.WriteLine("Using fallback English quotes - file not found or empty");
        }
        
        if (_spanishQuotes.Count == 0 && !File.Exists(ResolvePath(spanishPath)))
        {
            _spanishQuotes = GetFallbackSpanishQuotes();
            Console.WriteLine("Using fallback Spanish quotes - file not found or empty");
        }
    }

    public static string ResolvePath(string path)
    {
        if (Path.IsPathRooted(path))
            return path;
        
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDirectory, path);
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

    private List<Quote> LoadQuotes(string filePath)
    {
        try
        {
            var resolvedPath = ResolvePath(filePath);
            Console.WriteLine($"Attempting to load quotes from: {resolvedPath}");
            
            if (File.Exists(resolvedPath))
            {
                var json = File.ReadAllText(resolvedPath);
                Console.WriteLine($"File found, size: {json.Length} characters");
                
                try
                {
                    var collection = JsonSerializer.Deserialize<QuoteCollection>(json);
                    var quotes = collection?.Quotes ?? new List<Quote>();
                    Console.WriteLine($"Successfully parsed {quotes.Count} quotes from {resolvedPath}");
                    
                    // Debug: Show first quote if available
                    if (quotes.Count > 0)
                    {
                        Console.WriteLine($"First quote: {quotes[0].Text.Substring(0, Math.Min(50, quotes[0].Text.Length))}...");
                    }
                    
                    return quotes;
                }
                catch (JsonException jex)
                {
                    Console.WriteLine($"JSON parsing error for {resolvedPath}: {jex.Message}");
                    Console.WriteLine($"JSON content: {json}");
                }
            }
            else
            {
                Console.WriteLine($"File not found: {resolvedPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading {filePath}: {ex.Message}");
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