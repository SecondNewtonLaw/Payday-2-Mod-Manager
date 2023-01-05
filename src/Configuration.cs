using Newtonsoft.Json;
using Spectre.Console;

namespace ModManager;
[Serializable]
public class Configuration
{
    /// <summary>
    /// Payday 2 installation path, there are real uses for this, such as a Payday2 installation in another drive, where the steam folder is not the default.
    /// </summary>
    public string installPath;
    /// <summary>
    /// SuperBLT may use the same DLL to hook as Vanilla BLT did, marking this as true assures that the program considers this when checking the installed mod loader.
    /// </summary>
    public bool usingAlternativeSuperBLTDll;

    public Configuration()
    {
        // Set the default PD2 install path.
        installPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\PAYDAY 2\\";
        usingAlternativeSuperBLTDll = false;
    }

    public static string Serialize()
    {
        return JsonConvert.SerializeObject(Shared.ApplicationConfiguration);
    }

    public static Configuration Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<Configuration>(json)!;
    }

    public void SaveConfigurations(bool showConfigSaved = false)
    {
        try
        {
            File.WriteAllText("configuration.json", Serialize());
            if (showConfigSaved)
                AnsiConsole.MarkupLine("[green]Configurations saved[/]!");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Failed[/] to save configurations! Error: [yellow underline]{ex.Message}[/]");
        }
    }
    public static void LoadConfigurations()
    {
        try
        {
            string jsonRepresentation = File.ReadAllText("configuration.json");
            Shared.ApplicationConfiguration = JsonConvert.DeserializeObject<Configuration>(jsonRepresentation);
        }
        catch
        {
            Configuration cfg = new();
            Shared.ApplicationConfiguration = cfg;
            cfg.SaveConfigurations();
        }
    }
}