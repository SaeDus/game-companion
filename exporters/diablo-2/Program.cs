using System.Diagnostics;
using System.Text.Json;
using D2SSharp.Model;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("No command provided.");
            return 2;
        }

        string command = args[0].ToLowerInvariant();

        return command switch
        {
            "initialize" => InitializeReader(args),
            "export" => ExportCharacter(args),
            _ => UnknownCommand(command),
        };
    }

    private static int InitializeReader(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Error.WriteLine("Usage: d2-reader initialize <input_dir> <output_dir>");

            return 2;
        }

        string sourceDir = args[1];
        string outputDir = args[2];

        GeneratorResult result = GenerateDataFiles(sourceDir, outputDir).GetAwaiter().GetResult();

        Console.WriteLine(JsonSerializer.Serialize(result));

        return result.Success ? 0 : 1;
    }

    private static int ExportCharacter(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Error.WriteLine("Usage: d2-reader export <character_dir> <stash_dir>");

            return 2;
        }

        string pathToCharacter = args[1];
        string pathToStash = args[2];

        D2Save save = SaveLoader.LoadCharacter(pathToCharacter);
        D2StashSave stash = SaveLoader.LoadStash(pathToStash);

        GameState gameState = GameFormatter.BuildGameState(
            save,
            stash,
            pathToCharacter,
            pathToStash
        );

        Console.WriteLine(CharacterExporter.GetJsonFromGameState(gameState));
        Console.Error.WriteLine(StatFormatter.GetUnhandledStats());

        return 0;
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command: {command}");

        return 2;
    }

    private static async Task<GeneratorResult> GenerateDataFiles(string sourceDir, string outputDir)
    {
        string generatorPath = Path.Combine(AppContext.BaseDirectory, "generators", "generator.py");

        ProcessStartInfo startInfo = new()
        {
            FileName = "python",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        startInfo.ArgumentList.Add(generatorPath);
        startInfo.ArgumentList.Add(sourceDir);
        startInfo.ArgumentList.Add(outputDir);

        using Process process = new() { StartInfo = startInfo };

        process.Start();

        Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string stdout = await stdoutTask;
        string stderr = await stderrTask;

        if (string.IsNullOrWhiteSpace(stdout))
        {
            throw new Exception(
                $"Generator returned no JSON.\n"
                    + $"Exit code: {process.ExitCode}"
                    + $"stderr: {stderr}"
            );
        }

        GeneratorResult? result = JsonSerializer.Deserialize<GeneratorResult>(
            stdout,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return result is null
            ? throw new Exception(
                $"Generator returned invalid JSON.\n" + $"stdout: {stdout}\n" + $"stderr: {stderr}"
            )
            : result;
    }
}

public class Data
{
    public static string GetDataDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "game-companion",
            "games",
            "diablo-2",
            "data"
        );
    }
}
