using System.Collections.Concurrent;

namespace T1_Criptoanalise;

public class Program
{
    public static string alphabet = "abcdefghijklmnopqrstuvwxyz";

    public static double[] EnFreq =
    {
        0.08167, 0.01492, 0.02782, 0.04253, 0.12702, 0.02228, 0.02015, 0.06094, 0.06966, 0.00153, 0.00772, 0.04025,
        0.02406, 0.06749, 0.07507, 0.01929, 0.00095, 0.05987, 0.06327, 0.09056, 0.02758, 0.00978, 0.0236, 0.0015,
        0.01974, 0.00074
    };

    public static double[] PtFreq =
    {
        0.14634, 0.01043, 0.03882, 0.04992, 0.1257, 0.01023, 0.01303, 0.00781, 0.06186, 0.00397, 0.00015, 0.02779,
        0.04738, 0.04446, 0.09735, 0.02523, 0.01204, 0.0653, 0.06805, 0.04336, 0.03639, 0.01575, 0.00037, 0.00253,
        0.00006, 0.0047
    };

    public static void Main(string[] args)
    {
        CalculateDistances("Dados/TextoClaro.txt");
    }

    public static double CalculateDistances(string fileName)
    {
        var letterFrequencies = new ConcurrentDictionary<char, int>();
        string text = File.ReadAllText(fileName);

        Parallel.For(0, text.Length, i => { letterFrequencies.AddOrUpdate(text[i], 1, (_, v) => v + 1); });

        var textFrequencies = new List<double>();
        foreach (var letter in alphabet)
        {
            textFrequencies.Add(letterFrequencies[letter] / (double)text.Length);
            Console.WriteLine($"{letter} - {letterFrequencies[letter] / (double)text.Length}");
        }

        return 0;
    }

    public static int GCD(List<int> numbers)
    {
        int result = numbers[0];
        for (var i = 1; i < numbers.Count; i++)
        {
            result = GCD(result, numbers[i]);
        }

        return result;
    }

    public static int GCD(int a, int b)
    {
        while (b > 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }
}