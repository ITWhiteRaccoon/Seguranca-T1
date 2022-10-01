using System.Collections;
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
        var fileName = "Dados/Teste.txt";
        string text = File.ReadAllText(fileName);

        var columns = SplitText(text, 3);
        foreach (var column in columns)
        {
            Console.WriteLine(column);
        }
    }

    public static string CalculateLanguage(string text)
    {
        var teste = SplitText(text, 3);
        string mostFrequent = MostFrequentLetters(text);
        string probableLanguage = ProbableLanguage(mostFrequent);
        Console.WriteLine($"The text is probably in {probableLanguage}");

        var closestKeyLength = 0;
        var lastDistance = -1;

        Console.WriteLine("Text divided:");
        foreach (var str in teste)
        {
            Console.WriteLine(str);
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

    private static ConcurrentBag<string> SplitText(string text, int keyLength)
    {
        var result = new ConcurrentBag<string>();
        Parallel.For(0, text.Length / keyLength, i =>
        {
            var index = i * keyLength;
            result.Add(text[index..(index + keyLength)]);
        });

        return result;
    }
}