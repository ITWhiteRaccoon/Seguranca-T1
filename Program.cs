using System.Collections.Concurrent;

namespace T1_Criptoanalise;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(CalculateDistances("Dados/Textos cifrados/cipher1.txt"));
    }

    public static double CalculateDistances(string fileName)
    {
        var textCombinations = new ConcurrentDictionary<string, int>();
        string text = File.ReadAllText(fileName);

        Parallel.For(0, text.Length - 3, i => { textCombinations.AddOrUpdate(text[i..(i + 3)], 1, (_, v) => v + 1); });

        double sum = 0;
        foreach (int frequency in textCombinations.Values)
        {
            sum += frequency * (frequency - 1);
        }

        double coincidenceIndex = sum / ((long)text.Length * (text.Length - 1));

        
        
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