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
                Console.WriteLine($"JSON content: {json}");
                
                try
                {
                    // Try different JSON deserialization approaches
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip
                    };
                    
                    var collection = JsonSerializer.Deserialize<QuoteCollection>(json, options);
                    var quotes = collection?.Quotes ?? new List<Quote>();
                    
                    Console.WriteLine($"Successfully parsed {quotes.Count} quotes from {resolvedPath}");
                    
                    // Debug: Show all quotes
                    for (int i = 0; i < Math.Min(3, quotes.Count); i++)
                    {
                        Console.WriteLine($"Quote {i+1}: {quotes[i].Text.Substring(0, Math.Min(50, quotes[i].Text.Length))}...");
                    }
                    
                    return quotes;
                }
                catch (JsonException jex)
                {
                    Console.WriteLine($"JSON parsing error for {resolvedPath}: {jex.Message}");
                    Console.WriteLine($"LineNumber: {jex.LineNumber}, BytePosition: {jex.BytePositionInLine}");
                    
                    // Try manual parsing as fallback
                    return ParseQuotesManually(json);
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

    private List<Quote> ParseQuotesManually(string json)
    {
        try
        {
            // Simple manual parsing for basic JSON structure
            var quotes = new List<Quote>();
            
            // Look for "quotes" array
            var quotesStart = json.IndexOf("\"quotes\":[");
            if (quotesStart >= 0)
            {
                var arrayStart = json.IndexOf('[', quotesStart);
                var arrayEnd = json.LastIndexOf(']');
                
                if (arrayStart >= 0 && arrayEnd > arrayStart)
                {
                    var arrayContent = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
                    var quoteObjects = arrayContent.Split(new[] { "}," }, StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (var quoteObj in quoteObjects)
                    {
                        var cleanObj = quoteObj.Trim().TrimEnd('}');
                        
                        var textStart = cleanObj.IndexOf("\"text\":\"") + 8;
                        var textEnd = cleanObj.IndexOf("\"", textStart);
                        var authorStart = cleanObj.IndexOf("\"author\":\"") + 10;
                        var authorEnd = cleanObj.IndexOf("\"", authorStart);
                        
                        if (textStart > 7 && textEnd > textStart && authorStart > 9 && authorEnd > authorStart)
                        {
                            var text = cleanObj.Substring(textStart, textEnd - textStart);
                            var author = cleanObj.Substring(authorStart, authorEnd - authorStart);
                            
                            quotes.Add(new Quote { Text = text, Author = author });
                        }
                    }
                }
            }
            
            Console.WriteLine($"Manually parsed {quotes.Count} quotes");
            return quotes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Manual parsing failed: {ex.Message}");
            return new List<Quote>();
        }
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