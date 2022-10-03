using Spectre.Console;
using static System.Enum;

namespace T1_Criptoanalise;

public enum Language
{
    En,
    Pt
}

public class Program
{
    private static string InputFolder => "Dados";

    public static void Main(string[] args)
    {
        var files = Directory.GetFiles(InputFolder);
        for (var i = 0; i < files.Length; i++)
        {
            files[i] = files[i][(InputFolder.Length + 1)..];
        }

        var file = AnsiConsole.Prompt<string>(
            new SelectionPrompt<string>()
                .Title("Choose a [green]file[/]:")
                .AddChoiceGroup($"Listing files in [blue]{InputFolder}[/]", files)
        );
        Console.Clear();
        AnsiConsole.MarkupLine($"File selected: [blue]{InputFolder}/{file}[/]");

        var language = Parse<Language>(AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's the [green]language[/] of the file?")
                .AddChoices("Pt", "En")
        ));
        AnsiConsole.MarkupLine($"Language selected: [blue]{language}[/]");

        var text = File.ReadAllText($"{InputFolder}/{file}");

        var keyLength = 0;
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("Estimating key length...", _ => { keyLength = Vigenere.GetKeyLengthLevenshtein(text, language); });
        AnsiConsole.MarkupLine($"Estimated key length: [blue]{keyLength}[/]");

        Vigenere.GetKey(text, keyLength, language);

        var key = AnsiConsole.Prompt(
            new TextPrompt<string>("What [green]key[/] do you think it is?")
                .ValidationErrorMessage("[red]Key must include only letters and can't be empty.[/]")
                .Validate(key => key.All(char.IsLetter) && key.Length > 0)
        );
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"Chosen key: [blue]{key}[/]");

        var clearText = Vigenere.Decipher(text, key);
        File.WriteAllText($"{InputFolder}/OUT{file}", clearText);
        AnsiConsole.MarkupLine($"Wrote deciphered text to [blue]{InputFolder}/OUT{file}[/]");
        AnsiConsole.WriteLine($"First 100 characters: [{clearText[..100]}...]");
    }
}