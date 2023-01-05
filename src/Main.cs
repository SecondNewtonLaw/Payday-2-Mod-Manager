﻿using System;
using Spectre.Console;

namespace ModManager;

public static class MainActivity
{
    public static async Task Main()
    {
        const string PROGRAM_TITLE = "    PayDay 2 Mod Manager    ";
        string titleBorder = new('-', PROGRAM_TITLE.Length);
        AnsiConsole.MarkupLine($"\t|{titleBorder}|");
        AnsiConsole.MarkupLine($"\t|{PROGRAM_TITLE}|");
        AnsiConsole.MarkupLine($"\t|{titleBorder}|");

        Configuration.LoadConfigurations(); // Load all configurations.
        string paydayStatusString;
        try
        {
            if (Shared.ApplicationConfiguration!.usingAlternativeSuperBLTDll)
                paydayStatusString = ModManagement.CheckVanillaBLT() ? "Modded (SuperBLT)" : "Not Modded";
            else
                paydayStatusString = ModManagement.CheckSuperBLT() ? "Modded (SuperBLT)" : ModManagement.CheckVanillaBLT() ? "Modded (VanillaBLT)" : "Not Modded";
        }
        catch
        {
            paydayStatusString = "Not Found";
            AnsiConsole.MarkupLine($"\tPayday 2 Status: [yellow]{paydayStatusString}[/]");
            Shared.ApplicationConfiguration!.installPath = AnsiConsole.Ask<String>("We failed to automatically find Payday 2, please, paste the path of your game folder");
            if (!ModManagement.CheckGameInstall())
            {
                AnsiConsole.MarkupLine("[red]The given path does not lead to a valid Payday 2 installation, the program will now exit...[/]");
                Environment.Exit(-1);
            }
            Shared.ApplicationConfiguration.SaveConfigurations();

            if (Shared.ApplicationConfiguration!.usingAlternativeSuperBLTDll)
                paydayStatusString = ModManagement.CheckVanillaBLT() ? "Modded (SuperBLT)" : "Not Modded";
            else
                paydayStatusString = ModManagement.CheckSuperBLT() ? "Modded (SuperBLT)" : ModManagement.CheckVanillaBLT() ? "Modded (VanillaBLT)" : "Not Modded";
        }
        AnsiConsole.MarkupLine($"\tPayday 2 Status: [yellow]{paydayStatusString}[/]");
        AnsiConsole.MarkupLine("\t1. [green bold]Install[/] SuperBLT");
        AnsiConsole.MarkupLine("\t2. [yellow]Modify[/] Settings");
        AnsiConsole.MarkupLine("\t3. [red]Exit[/]");

        int choice = AnsiConsole.Prompt(new TextPrompt<int>($"Select an option [red]@[underline]{Environment.UserName}[/][/]")
        {
            PromptStyle = new(null, null, Decoration.SlowBlink, null),
            IsSecret = false,
            ValidationErrorMessage = "That is not a valid choice.",
            ShowChoices = false,
            AllowEmpty = false,
        }.AddChoices(new int[] { 1, 2, 3 }));

        switch (choice)
        {
            case 1:
                bool useOldHookDll = AnsiConsole.Confirm("Use old hooking dll? (This will download the IPHLPAPI.dll SuperBLT instead of the WSOCK32.dll)", false);
                Shared.ApplicationConfiguration!.usingAlternativeSuperBLTDll = useOldHookDll;
                Shared.ApplicationConfiguration.SaveConfigurations();
                bool success = await ModManagement.InstallSuperBlt(useOldHookDll);

                if (!success)
                {
                    AnsiConsole.MarkupLine("SuperBLT Installation failed!");
                }
                break;

            case 2:
                break;

            case 3:
                Environment.Exit(0);
                break;

            default:
                Environment.Exit(-1);
                break;
        }
    }
}