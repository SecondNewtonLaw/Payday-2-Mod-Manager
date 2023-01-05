using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Spectre.Console;

namespace ModManager;

public partial class ModManagement
{
    /// <summary>
    /// URI for the WSOCK32.dll SuperBLT hooking variant.
    /// </summary>
    const string SUPERBLT_DLL_DOWNLOAD_WSOCK32 = "https://znix.xyz/random/payday-2/SuperBLT/latest-wsock.php";
    /// <summary>
    /// URI for the IPHLPAPI.dll SuperBLT hooking variant.
    /// </summary>
    const string SUPERBLT_DLL_DOWNLOAD_IPHLPAPI = "https://znix.xyz/random/payday-2/SuperBLT/latest-release.php";
    /// <summary>
    /// URI for the Base Mod files for SuperBLT.
    /// </summary>
    const string SUPERBLT_BASEMOD_FILES = "https://znix.xyz/paydaymods/misc/SuperBLT/dll_download_endpoint.php";
    /// <summary>
    /// Installs SuperBLT along with it's base files onto the game installation directory.
    /// </summary>
    /// <param name="useOldHookingDll">Specified wether when installing the hooking dll into the game directory the IPHLPAPI.dll variant should be used.</param>
    /// <returns>True if SuperBLT was installed successfully, else, False.</returns>
    public static async Task<bool> InstallSuperBlt(bool useOldHookingDll = false)
    {
        Task<bool> dllInstall = Task.Run(async () =>
        {
            string tempFile = Path.GetTempFileName();
            HttpResponseMessage? dllDownload = null;
            try
            {
                string dllVariant = useOldHookingDll ? "IPHLPAPI" : "WSOCK32";
                dllDownload =
                useOldHookingDll ? await Shared.HttpClient.GetAsync(SUPERBLT_DLL_DOWNLOAD_IPHLPAPI) // Gets the IPHLPAPI.dll SuperBLT variant if requested in the method parameters.
                                : await Shared.HttpClient.GetAsync(SUPERBLT_DLL_DOWNLOAD_WSOCK32);

                AnsiConsole.MarkupLine($"(SuperBLT dll) [yellow]Downloading SuperBLT Dll[/], [red]{dllVariant}[/] variant. ({dllDownload.Content.Headers.ContentLength} bytes)");

                Stream dllStream = await dllDownload.Content.ReadAsStreamAsync();
                FileStream handle = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                await dllStream.CopyToAsync(handle);
                await dllStream.DisposeAsync();
                await handle.FlushAsync();
                await handle.DisposeAsync();

                AnsiConsole.MarkupLine("(SuperBLT dll) [yellow]Installing SuperBLT.[/]");
                // The DLL download is actually a Zip file, extract its contents into the game folder.
                ZipFile.ExtractToDirectory(tempFile, Shared.ApplicationConfiguration.installPath);
                AnsiConsole.MarkupLine("(SuperBLT dll) [green]SuperBLT installed![/]");
                return true;
            }
            catch { return false; }
            finally
            {
                AnsiConsole.MarkupLine("(SuperBLT dll) [yellow]Cleaning Up...[/]");
                // Delete temporal file.
                File.Delete(tempFile);
                dllDownload?.Dispose();
                AnsiConsole.MarkupLine("(SuperBLT dll) [green]Done.[/]");
            }
        });

        Task<bool> superBltBase = Task.Run(async () =>
        {
            string tempFile = Path.GetTempFileName();
            HttpResponseMessage? baseModFiles = null;
            try
            {
                string modsDirectory = Path.Combine(Shared.ApplicationConfiguration.installPath, "mods");
                AnsiConsole.MarkupLine("(Mod Files) Creating folders...");
                Directory.CreateDirectory(modsDirectory);

                baseModFiles = await Shared.HttpClient.GetAsync(SUPERBLT_BASEMOD_FILES);
                AnsiConsole.MarkupLine($"(Mod Files) [yellow]Downloading SuperBLT BaseMod files[/]. ({baseModFiles.Content.Headers.ContentLength} bytes)");

                Stream baseFilesStream = await baseModFiles.Content.ReadAsStreamAsync();

                AnsiConsole.MarkupLine("(Mod Files) [yellow]Installing SuperBLT Base Mod files.[/]");

                FileStream handle = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                await baseFilesStream.CopyToAsync(handle);
                await baseFilesStream.DisposeAsync();
                await handle.FlushAsync();
                await handle.DisposeAsync();
                ZipFile.ExtractToDirectory(tempFile, modsDirectory);
                AnsiConsole.MarkupLine("(Mod Files) [green]SuperBLT Base Mod files installed![/]");

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                AnsiConsole.MarkupLine("(Mod Files) [yellow]Cleaning Up...[/]");
                // Delete temporal file.
                File.Delete(tempFile);
                baseModFiles?.Dispose();
                AnsiConsole.MarkupLine("(Mod Files) [green]Done.[/]");
            }
        });
        Task.WaitAll(new Task[] { superBltBase, dllInstall });

        // Tasks are already resolved, no deadlock risk.
        return dllInstall.Result && superBltBase.Result;
    }
}
