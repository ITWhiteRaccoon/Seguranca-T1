using System.Collections.Concurrent;
using System.Text;
using DuoVia.FuzzyStrings;
using Spectre.Console;

namespace T1_Criptoanalise;

public class Vigenere
{
    private static string Alphabet => "abcdefghijklmnopqrstuvwxyz";
    private static string MostFrequentLettersEn => "etaoinshrdlu";
    private static string MostFrequentLettersPt => "aeosridmntcu";
    private static double CoincidenceIndexEn => 0.0661;
    private static double CoincidenceIndexPt => 0.0738;
    private static int MaxKeyLength => 26;

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

    public static string Decipher(string text, string key)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < text.Length; i++)
        {
            var letter = text[i];
            var keyLetter = key[i % key.Length];
            var newLetter =
                Alphabet[(Alphabet.IndexOf(letter) - Alphabet.IndexOf(keyLetter) + Alphabet.Length) % Alphabet.Length];
            sb.Append(newLetter);
        }

        return sb.ToString();
    }

    public static char[,] GetKey(string text, int keyLength, Language language)
    {
        var targetMostFrequent = language == Language.Pt ? MostFrequentLettersPt : MostFrequentLettersEn;
        var separatedText = SplitText(text, keyLength);
        var probableKey = new char[keyLength, 2];
        for (var i = 0; i < keyLength; i++)
        {
            var mostFrequent = GetMostFrequentLetters(GetLetterFrequency(separatedText[i]));
            for (var j = 0; j < 2; j++)
            {
                probableKey[i, j] = Alphabet[
                    Alphabet.IndexOf(mostFrequent[j]) - Alphabet.IndexOf(targetMostFrequent[0]) +
                    Alphabet.Length % Alphabet.Length];
            }

            AnsiConsole.MarkupLine(
                $"Position [blue]{i}[/] of key could be [blue]{probableKey[i, 0]}[/], [blue]{probableKey[i, 1]}[/]");
        }

        return probableKey;
    }

    public static int GetKeyLengthLevenshtein(string text, Language language)
    {
        var targetFrequency = language == Language.Pt ? MostFrequentLettersPt : MostFrequentLettersEn;

        var keyLength = 0;
        var closestDistance = -1.0;
        var bestDistances = new double[MaxKeyLength];

        Parallel.For(0, MaxKeyLength, i =>
        {
            var separatedText = SplitText(text, i + 1);
            var averageDistance = 0.0;
            foreach (var column in separatedText)
            {
                var letterFrequency = GetLetterFrequency(column);
                var mostFrequentLetters = GetMostFrequentLetters(letterFrequency);
                averageDistance += targetFrequency.LevenshteinDistance(mostFrequentLetters);
            }

            bestDistances[i] = averageDistance / separatedText.Length;
        });

        for (var i = 0; i < MaxKeyLength; i++)
        {
            if (bestDistances[i] < closestDistance || closestDistance == -1)
            {
                closestDistance = bestDistances[i];
                keyLength = i + 1;
            }
        }

        return keyLength;
    }

    private static int GetKeyLengthFriedman(string text, Language language)
    {
        var targetCoincidenceIndex = language == Language.Pt ? CoincidenceIndexPt : CoincidenceIndexEn;

        var keyLength = 0;
        var closest = 0.0;
        var coincidenceIndexes = new double[MaxKeyLength];
        for (var i = 0; i < MaxKeyLength; i++)
        {
            coincidenceIndexes[i] = GetAverageCoincidenceIndex(SplitText(text, i + 1));
        }

        for (var i = 0; i < MaxKeyLength; i++)
        {
            if (Math.Abs(targetCoincidenceIndex - coincidenceIndexes[i]) < Math.Abs(targetCoincidenceIndex - closest))
            {
                closest = coincidenceIndexes[i];
                keyLength = i + 1;
            }
        }

        return keyLength;
    }

    private static double GetAverageCoincidenceIndex(ConcurrentBag<char>[] separatedText)
    {
        var coincidenceIndexes = new ConcurrentBag<double>();

        foreach (var t in separatedText)
        {
            coincidenceIndexes.Add(GetCoincidenceIndex(t));
        }

        return coincidenceIndexes.Sum() / separatedText.Length;
    }

    private static double GetCoincidenceIndex(ConcurrentBag<char> text)
    {
        var frequencies = GetLetterFrequency(text);
        var coincidenceIndex = 0.0;
        foreach (var letterFrequency in frequencies.Values)
        {
            coincidenceIndex += letterFrequency * (letterFrequency - 1);
        }

        return coincidenceIndex / ((double)text.Count * (text.Count - 1));
    }

    private static string GetMostFrequentLetters(IDictionary<char, int> letterFrequencies)
    {
        //Take each letter frequency, add to list, and order by most frequent.
        var textFrequencies = new List<LetterFrequency>();
        foreach (var pair in letterFrequencies)
        {
            textFrequencies.Add(new LetterFrequency { Letter = pair.Key, Frequency = pair.Value });
        }

        textFrequencies.Sort((x, y) => y.Frequency.CompareTo(x.Frequency));

        //Take the 12 most frequent letters or less. (We will use these later to see how close they are with the most frequent letters in english and portuguese.)
        StringBuilder mostFrequent = new();
        var limit = textFrequencies.Count < 12 ? textFrequencies.Count : 12;
        for (var i = 0; i < limit; i++)
        {
            mostFrequent.Append(textFrequencies[i].Letter);
        }

        return mostFrequent.ToString();
    }

    private static ConcurrentDictionary<char, int> GetLetterFrequency(IEnumerable<char> text)
    {
        var letterFrequencies = new ConcurrentDictionary<char, int>();
        foreach (var c in text)
        {
            letterFrequencies.AddOrUpdate(c, 1, (_, v) => v + 1);
        }

        return letterFrequencies;
    }

    private static ConcurrentBag<char>[] SplitText(string text, int keyLength)
    {
        var result = new ConcurrentBag<char>[keyLength];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = new ConcurrentBag<char>();
        }

        for (var i = 0; i < text.Length; i++)
        {
            result[i % keyLength].Add(text[i]);
        }

        return result;
    }
}