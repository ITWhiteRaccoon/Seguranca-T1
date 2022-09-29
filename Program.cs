using System.Collections.Concurrent;
using System.Text;
using DuoVia.FuzzyStrings;

namespace T1_Criptoanalise;

public class Program
{
    class LetterFrequency : IComparable<LetterFrequency>
    {
        public char Letter { get; set; }
        public int Frequency { get; set; }

        public int CompareTo(LetterFrequency other)
        {
            return other.Frequency.CompareTo(Frequency);
        }

        public override string ToString()
        {
            return $"{Letter} - {Frequency}";
        }
    }

    public static string alphabet = "abcdefghijklmnopqrstuvwxyz";

    public static string enMostFreq = "etaoinshrdlu";
    public static string ptMostFreq = "aeosridmntcu";

    public static void Main(string[] args)
    {
        CalculateLanguage("Dados/TextoClaro.txt");
    }

    public static string CalculateLanguage(string fileName)
    {
        string text = File.ReadAllText(fileName);

        string mostFrequent = MostFrequentLetters(text);
        string probableLanguage = ProbableLanguage(mostFrequent);
        Console.WriteLine($"The text is probably in {probableLanguage}");

        var closestKeyLength = 0;
        var lastDistance = -1;

        for (int i = 1; i < 26; i++)
        {
            
        }

        return probableLanguage;
    }

    private static string ProbableLanguage(string mostFrequent)
    {
        Console.WriteLine($"Most frequent letters in text {mostFrequent}");
        double enDistance = mostFrequent.LevenshteinDistance(enMostFreq);
        double ptDistance = mostFrequent.LevenshteinDistance(ptMostFreq);
        Console.WriteLine($"distance to english: {enDistance}");
        Console.WriteLine($"distance to portuguese: {ptDistance}");
        return enDistance < ptDistance ? "english" : "portuguese";
    }

    private static string MostFrequentLetters(string text)
    {
        var letterFrequencies = new ConcurrentDictionary<char, int>();
        Parallel.For(0, text.Length, i => { letterFrequencies.AddOrUpdate(text[i], 1, (_, v) => v + 1); });

        var textFrequencies = new List<LetterFrequency>();
        foreach (var letterFrequency in letterFrequencies)
        {
            textFrequencies.Add(new LetterFrequency
            {
                Letter = letterFrequency.Key,
                Frequency = letterFrequency.Value
            });
        }

        textFrequencies.Sort((x, y) => y.Frequency.CompareTo(x.Frequency));
        Console.WriteLine(string.Join('\n', textFrequencies));

        var mostFrequent = new StringBuilder();
        for (int i = 0; i < 12; i++)
        {
            mostFrequent.Append(textFrequencies[i].Letter);
        }

        return mostFrequent.ToString();
    }
}