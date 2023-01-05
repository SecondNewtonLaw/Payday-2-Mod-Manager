namespace ModManager;

public partial class ModManagement
{
    const string PAYDAY2_EXE = "payday2_win32_release.exe";
    /// <summary>
    /// Verifies if the game is installed by checking some things.
    /// </summary>
    /// <returns>True if the game installation is valid, else False.</returns>
    public static bool CheckGameInstall()
    {
        return
            Directory.Exists(Shared.ApplicationConfiguration!.installPath) && // Check for the game folder.
            File.Exists(Path.Combine(Shared.ApplicationConfiguration!.installPath, PAYDAY2_EXE)) && // Check for the Payday 2 Exe.
            Directory.Exists(Path.Combine(Shared.ApplicationConfiguration!.installPath, "assets")); // Check for the game assets.
    }
}