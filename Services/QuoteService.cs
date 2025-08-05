using System.Text.Json;
using System.IO;

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
    private readonly List<Quote> _englishQuotes;
    private readonly List<Quote> _spanishQuotes;
    private readonly Random _random = new();

    public QuoteService()
    {
        _englishQuotes = LoadQuotes("english_quotes.json");
        _spanishQuotes = LoadQuotes("spanish_quotes.json");
    }

    private List<Quote> LoadQuotes(string fileName)
    {
        try
        {
            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                var collection = JsonSerializer.Deserialize<QuoteCollection>(json);
                return collection?.Quotes ?? new List<Quote>();
            }
        }
        catch (Exception ex)
        {
            // Log error or return empty list
            Console.WriteLine($"Error loading {fileName}: {ex.Message}");
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