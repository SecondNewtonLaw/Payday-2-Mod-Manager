using System.IO.Compression;
using Masked.SpectreConsole;
using Masked.SpectreConsole.Extensions;
using Masked.Sys.Extensions;
using Spectre.Console;

namespace ModManager;

public partial class ModManagement
{
    public static async Task<bool> InstallModAsync(Uri modUri)
    {
        DownloadBarItem item = CreateDownloadItem(modUri);

        DownloadBar dlBar = new(Shared.HttpClient);

        Dictionary<Stream, DownloadBarItem>? dlResult = (Dictionary<Stream, DownloadBarItem>)await dlBar.StartDownloadBar(new DownloadBarItem[] { item });

        dlResult.FastIterator((kvp, _) =>
        {
            kvp.Key.Flush();
            kvp.Key.Dispose();
            AnsiConsole.MarkupLine($"[yellow]Installing {kvp.Value.ItemName}...[/]");
            ZipFile.ExtractToDirectory(kvp.Value.SavePath, Path.Combine(Shared.ApplicationConfiguration.installPath, "mods"));
            AnsiConsole.MarkupLine($"[yellow]{kvp.Value.ItemName} has been installed![/]");
            File.Delete(kvp.Value.SavePath);
            return NextStep.Continue;
        });
        return true;
    }
    public static async Task<bool> InstallModsAsync(params Uri[] mods)
    {
        DownloadBarItem[] items = new DownloadBarItem[mods.Length];
        mods.FastIterator((uri, index) =>
        {
            items[index] = CreateDownloadItem(uri);
            return NextStep.Continue;
        });

        DownloadBar dlBar = new(Shared.HttpClient);

        Dictionary<Stream, DownloadBarItem> dlResult = (Dictionary<Stream, DownloadBarItem>)await dlBar.StartDownloadBar(items);

        dlResult.FastIterator((kvp, _) =>
        {
            kvp.Key.Flush();
            kvp.Key.Dispose();
            AnsiConsole.MarkupLine($"[yellow]Installing {kvp.Value.ItemName}...[/]");
            ZipFile.ExtractToDirectory(kvp.Value.SavePath, Path.Combine(Shared.ApplicationConfiguration.installPath, "mods"));
            AnsiConsole.MarkupLine($"[yellow]{kvp.Value.ItemName} has been installed![/]");
            File.Delete(kvp.Value.SavePath);
            return NextStep.Continue;
        });
        return true;
    }
    public static async Task<bool> InstallModAsync(string zipPath)
    {
        return await InstallModFromZip(zipPath);
    }
    private static Task<bool> InstallModFromZip(string zipPath)
    {
        try
        {
            ZipFile.ExtractToDirectory(zipPath, Path.Combine(Shared.ApplicationConfiguration.installPath, "mods"));
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private static DownloadBarItem CreateDownloadItem(Uri zipSourceUri)
    {
        return new()
        {
            ItemName = zipSourceUri.OriginalString.Split('/').Last().Split('.')[0],
            SavePath = Path.Combine(Path.GetTempPath(), zipSourceUri.OriginalString.Split('/').Last()),
            Url = zipSourceUri
        };
    }
}