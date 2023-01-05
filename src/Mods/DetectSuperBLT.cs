namespace ModManager;

public partial class ModManagement
{
    /// <summary>
    /// The name of the dll that SuperBLT hooks as.
    /// </summary>
    public const string SUPER_BLT_DLL = "WSOCK32.dll";
    /// <summary>
    /// The name of the dll that vanilla BLT hooks as.
    /// </summary>
    public const string BLT_DLL = "IPHLPAPI.dll";
    /// <summary>
    /// Checks for the modified SuperBLT dll to see if it is installed.
    /// </summary>
    /// <returns>True if the SuperBLT dll is found, else False.</returns>
    public static bool CheckSuperBLT()
    {
        if (!Directory.Exists(Shared.ApplicationConfiguration!.installPath))
            throw new GameNotFoundException($"Could not found a Payday 2 installation in path \"{Shared.ApplicationConfiguration!.installPath}\".");

        return File.Exists(Path.Combine(Shared.ApplicationConfiguration!.installPath, SUPER_BLT_DLL));
    }
    /// <summary>
    /// Checks for the modified Vanilla BLT dll to see if it is installed.
    /// </summary>
    /// <returns>True if the Vanilla BLT dll is found, else False.</returns>
    public static bool CheckVanillaBLT()
    {
        if (!Directory.Exists(Shared.ApplicationConfiguration!.installPath))
            throw new GameNotFoundException($"Could not found a Payday 2 installation in path \"{Shared.ApplicationConfiguration!.installPath}\".");

        return File.Exists(Path.Combine(Shared.ApplicationConfiguration!.installPath, BLT_DLL));
    }
}