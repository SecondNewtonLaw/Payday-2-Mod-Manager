using System.IO.Compression;
using System.Net.Mime;
using Masked.SpectreConsole;
using Masked.SpectreConsole.Extensions;
using Masked.Sys.Extensions;
using Newtonsoft.Json;
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
            try
            {
                string bltModsDirectory = Path.Combine(Shared.ApplicationConfiguration.installPath, "mods");
                string modOverridesDirectory = Path.Combine(Shared.ApplicationConfiguration.installPath, "assets", "mod_overrides");
                string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                ZipFile.ExtractToDirectory(kvp.Value.SavePath, tempFolder);

                // We want to get if the mod is a BLT mod or not, this can be simplified to searching a file like mod.txt in the underlying folder structure.
                string[] subFolders = Directory.GetDirectories(tempFolder);
                bool foundBlt = false;

                string[] files = Directory.GetFiles(subFolders[0]);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i] is "mod.txt")
                    {
                        Console.WriteLine(files[i]);
                        foundBlt = true;
                        break;
                    }
                }

                if (foundBlt)
                    AnsiConsole.MarkupLine($"[yellow]{kvp.Value.ItemName} is a BLT Mod.[/]");
                else
                    AnsiConsole.MarkupLine($"[yellow]{kvp.Value.ItemName} is a Mod Override.[/]");

                // Copy mod if it was a BLT or override mod..
                // NOTE: Moving files across different volumes (ig C, E, D) will FAIL.
                if (Shared.ApplicationConfiguration.installPath[0] != subFolders[0][0])
                    ZipFile.ExtractToDirectory(kvp.Value.SavePath, foundBlt ? bltModsDirectory : modOverridesDirectory);
                else
                    Directory.Move(subFolders[0], foundBlt ? bltModsDirectory : modOverridesDirectory);
                Directory.Delete(tempFolder); // Delete the folder, it's empty theoretically.

                AnsiConsole.MarkupLine($"[green]{kvp.Value.ItemName} has been installed![/]");
            }
            catch (IOException ioEx)
            {
                AnsiConsole.MarkupLine($"[red]{kvp.Value.ItemName} has failed on its installation. Error: {ioEx.Message}[/]");
            }
            finally
            {
                File.Delete(kvp.Value.SavePath);
            }
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
            try
            {
                string bltModsDirectory = Path.Combine(Shared.ApplicationConfiguration.installPath, "mods");
                string modOverridesDirectory = Path.Combine(Shared.ApplicationConfiguration.installPath, "assets", "mod_overrides");
                string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                ZipFile.ExtractToDirectory(kvp.Value.SavePath, tempFolder);

                // We want to get if the mod is a BLT mod or not, this can be simplified to searching a file like mod.txt in the underlying folder structure.
                string[] subFolders = Directory.GetDirectories(tempFolder);
                bool foundBlt = false;

                string[] files = Directory.GetFiles(subFolders[0]);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i] is "mod.txt")
                    {
                        Console.WriteLine(files[i]);
                        foundBlt = true;
                        break;
                    }
                }

                if (foundBlt)
                    AnsiConsole.MarkupLine($"[yellow]{kvp.Value.ItemName} is a BLT Mod.[/]");
                else
                    AnsiConsole.MarkupLine($"[yellow]{kvp.Value.ItemName} is a Mod Override.[/]");

                // Copy mod if it was a BLT or override mod..
                // NOTE: Moving files across different volumes (ig C, E, D) will FAIL.
                if (Shared.ApplicationConfiguration.installPath[0] != subFolders[0][0])
                    ZipFile.ExtractToDirectory(kvp.Value.SavePath, foundBlt ? bltModsDirectory : modOverridesDirectory);
                else
                    Directory.Move(subFolders[0], foundBlt ? bltModsDirectory : modOverridesDirectory);
                Directory.Delete(tempFolder); // Delete the folder, it's empty theoretically.

                AnsiConsole.MarkupLine($"[green]{kvp.Value.ItemName} has been installed![/]");
            }
            catch (IOException ioEx)
            {
                AnsiConsole.MarkupLine($"[red]{kvp.Value.ItemName} has failed on its installation. Error: {ioEx.Message}[/]");
            }
            finally
            {
                File.Delete(kvp.Value.SavePath);
            }
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
            string bltModsDirectory = Path.Combine(Shared.ApplicationConfiguration.installPath, "mods");
            string modOverridesDirectory = Path.Combine(Shared.ApplicationConfiguration.installPath, "assets", "mod_overrides");
            string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            ZipFile.ExtractToDirectory(zipPath, tempFolder);

            // We want to get if the mod is a BLT mod or not, this can be simplified to searching a file like mod.txt in the underlying folder structure.
            string[] subFolders = Directory.GetDirectories(tempFolder);
            bool foundBlt = false;

            string[] files = Directory.GetFiles(subFolders[0]);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] is "mod.txt")
                {
                    Console.WriteLine(files[i]);
                    foundBlt = true;
                    break;
                }
            }

            if (foundBlt)
                AnsiConsole.MarkupLine("[yellow]The Mod is a BLT Mod.[/]");
            else
                AnsiConsole.MarkupLine("[yellow]The Mod is a Mod Override.[/]");

            // Copy mod if it was a BLT or override mod..
            // NOTE: Moving files across different volumes (ig C, E, D) will FAIL.
            if (Shared.ApplicationConfiguration.installPath[0] != subFolders[0][0])
                ZipFile.ExtractToDirectory(zipPath, foundBlt ? bltModsDirectory : modOverridesDirectory);
            else
                Directory.Move(subFolders[0], foundBlt ? bltModsDirectory : modOverridesDirectory);
            Directory.Delete(tempFolder); // Delete the folder, it's empty theoretically.

            AnsiConsole.MarkupLine($"[green]The Mod has been installed![/]");
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private static DownloadBarItem CreateDownloadItem(Uri zipSourceUri)
    {
        // modworkshop.net makes redirects, if we don't do some extra things we will download an html instead of a zip.
        // we don't want to modify the URL if it already points to their api.
        if (zipSourceUri.Host == "modworkshop.net" && !zipSourceUri.AbsoluteUri.Contains("/api/files/", StringComparison.InvariantCultureIgnoreCase))
        {
            zipSourceUri = new($"https://modworkshop.net/api/files/{zipSourceUri.AbsoluteUri.Split('/').Last()}/download");
        }
        HttpRequestMessage request = new(HttpMethod.Head, zipSourceUri);
        HttpResponseMessage headReq = Shared.HttpClient.Send(request); // Get the headers, allows us to know the filename.
        string itemName;
        try
        {
            itemName = headReq.Content.Headers.ContentDisposition?.FileName!;

            itemName ??= zipSourceUri.OriginalString.Split('/').Last().Split('.')[0] + $"({Guid.NewGuid()})";
        }
        catch
        {
            itemName = $"Unknown Mod ({Guid.NewGuid()})";
        }
        return new()
        {
            ItemName = itemName,
            SavePath = Path.GetTempFileName(),
            Url = zipSourceUri
        };
    }
}