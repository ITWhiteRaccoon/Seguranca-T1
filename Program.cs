using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using DuoVia.FuzzyStrings;

namespace T1_Criptoanalise;

public enum Language
{
    En,
    Pt
}

public class Program
{
    private static string Alphabet { get; } = "abcdefghijklmnopqrstuvwxyz";
    private static string MostFrequentLettersEn { get; } = "etaoinshrdlu";
    private static string MostFrequentLettersPt { get; } = "aeosridmntcu";
    private static double CoincidenceIndexEn = 0.0661;
    private static double CoincidenceIndexPt = 0.0738;
    private static double CoincidenceIndexRandom = 0.0385;
    private static int MaxKeyLength { get; } = 26;

    private class LetterFrequency : IComparable<LetterFrequency>
    {
        public char Letter { get; set; }
        public int Frequency { get; set; }

        public int CompareTo(LetterFrequency? other)
        {
            return other!.Frequency.CompareTo(Frequency);
        }

        public override string ToString()
        {
            return $"{Letter} - {Frequency}";
        }
    }

    public static void Main(string[] args)
    {
        string text = File.ReadAllText("Dados/Textos cifrados/cipher1.txt");

        RunAuto(text);
    }

    private static void RunAuto(string text)
    {
        //var frequency = GetLetterFrequency(text);
        //var language = GetLanguage(frequency);
        var language = Language.Pt;
        Console.WriteLine(language);

        var keyLength = 0;
        var closest = 0;
        var coincidenceIndex = new double[MaxKeyLength];
        for (int i = 0; i < MaxKeyLength; i++)
        {
            var split
            coincidenceIndex[i]= GetCoincidenceIndex();
        }
    }
    
    private static double GetCoincidenceIndex(ICollection<string> brokenText)
    {
        var result = 0.0;
        var coincidenceIndexes = new ConcurrentBag<double>();
        Parallel.ForEach(brokenText, (subStr) =>
        {
            var letterFrequency = GetLetterFrequency(subStr);
            var subStrCoincidenceIndexes = new ConcurrentBag<double>();
            Parallel.ForEach(Alphabet, (letter) =>
            {
                subStrCoincidenceIndexes.Add();
            });
            coincidenceIndexes.Add();
        });
        return result/brokenText.Count;
    }

    private static Language GetLanguage(IDictionary<char, int> letterFrequencies)
    {
        //Get a string with the first (most relevant) most frequent letters.
        string mostFrequent = GetMostFrequentLetters(letterFrequencies);
        Console.WriteLine($"Most frequent letters in text {mostFrequent}");

        //Use Levenshtein distance to calculate how close the most frequent letters are to the most frequent letters of each language.
        double distanceToEn = mostFrequent.LevenshteinDistance(MostFrequentLettersEn);
        double distanceToPt = mostFrequent.LevenshteinDistance(MostFrequentLettersPt);
        Console.WriteLine($"Distance to english: {distanceToEn}");
        Console.WriteLine($"Distance to portuguese: {distanceToPt}");

        return distanceToEn < distanceToPt ? Language.En : Language.Pt;
    }

    private static string GetMostFrequentLetters(IDictionary<char, int> letterFrequencies)
    {
        //Take each letter frequency, add to list, and order by most frequent.
        var textFrequencies = new List<LetterFrequency>();
        Parallel.ForEach(letterFrequencies,
            pair => { textFrequencies.Add(new LetterFrequency { Letter = pair.Key, Frequency = pair.Value }); });
        textFrequencies.Sort((x, y) => y.Frequency.CompareTo(x.Frequency));

        //Take the 12 most frequent letters or less. (We will use these later to see how close they are with the most frequent letters in english and portuguese.)
        StringBuilder mostFrequent = new();
        int limit = textFrequencies.Count < 12 ? textFrequencies.Count : 12;
        for (var i = 0; i < limit; i++)
        {
            mostFrequent.Append(textFrequencies[i].Letter);
        }

        return mostFrequent.ToString();
    }

    private static ConcurrentDictionary<char, int> GetLetterFrequency(string text)
    {
        var letterFrequencies = new ConcurrentDictionary<char, int>();
        Parallel.For(0, text.Length, i => { letterFrequencies.AddOrUpdate(text[i], 1, (_, v) => v + 1); });
        return letterFrequencies;
    }

    private static ConcurrentBag<string> SplitText(string text, int keyLength)
    {
        
        var result = new ConcurrentBag<string>();
        Parallel.For(0, text.Length / keyLength, i =>
        {
            int index = i * keyLength;
            result.Add(text[index..(index + keyLength)]);
        });

        return result;
    }
}