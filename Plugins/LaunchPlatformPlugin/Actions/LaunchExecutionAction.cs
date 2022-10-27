using Frosty.Core;
using Frosty.ModSupport;
using FrostySdk;
using FrostySdk.Interfaces;
using LaunchPlatformPlugin.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace LaunchPlatformPlugin.Actions
{
    public abstract class Platform
    {
        /// <summary>
        /// The installation path of the platform.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The name of the process when opened.
        /// </summary>
        public abstract string ProcessName { get; }

        /// <summary>
        /// The registry key for the platform, used to get the installation path.
        /// </summary>
        public abstract string RegistryKey { get; }

        /// <summary>
        /// The registry value for the platform, used to get the installation path.
        /// </summary>
        public abstract string RegistryValue { get; }

        /// <summary>
        /// Appended onto the registry path if it doesn't include the direct executable path.
        /// </summary>
        public abstract string AdditionalExecutablePath { get; }
    }

    public class OriginPlatform : Platform
    {
        public override string ProcessName => "Origin";
        public override string RegistryKey => "Electronic Arts\\EA Core";
        public override string RegistryValue => "EADM6InstallDir";
        public override string AdditionalExecutablePath => "\\Origin.exe";
    }
    public class EADesktopPlatform : Platform
    {
        public override string ProcessName => "EADesktop";
        public override string RegistryKey => "Electronic Arts\\EA Desktop";
        public override string RegistryValue => "DesktopAppPath";
        public override string AdditionalExecutablePath => "";
    }
    public class SteamPlatform : Platform
    {
        public override string ProcessName => "steam";
        public override string RegistryKey => "Valve\\Steam";
        public override string RegistryValue => "InstallPath";
        public override string AdditionalExecutablePath => "\\steam.exe";
    }
    public class EpicGamesPlatform : Platform
    {
        public override string ProcessName => "EpicGamesLauncher";
        public override string RegistryKey => "EpicGames\\Unreal Engine";
        public override string RegistryValue => "INSTALLDIR";
        public override string AdditionalExecutablePath => "\\Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe";
    }

    public class LaunchExecutionAction : ExecutionAction
    {

        private Dictionary<LaunchPlatform, Platform> platforms = new Dictionary<LaunchPlatform, Platform>
        {
            { LaunchPlatform.Origin, new OriginPlatform() },
            { LaunchPlatform.EADesktop, new EADesktopPlatform() },
            { LaunchPlatform.Steam, new SteamPlatform() },
            { LaunchPlatform.EpicGamesLauncher, new EpicGamesPlatform() },
        };
        private Platform selectedPlatform;
        private bool relaunchPlatform = false;

        public override Action<ILogger, PluginManagerType, CancellationToken> PreLaunchAction => new Action<ILogger, PluginManagerType, CancellationToken>((ILogger logger, PluginManagerType type, CancellationToken cancelToken) =>
        {
            // only run platform launch system when PlatformLaunchingEnabled is true
            if (Config.Get("PlatformLaunchingEnabled", false, ConfigScope.Game))
            {
                LaunchPlatform platform = (LaunchPlatform)Enum.Parse(typeof(LaunchPlatform), Config.Get("Platform", "Origin", ConfigScope.Game));
                selectedPlatform = platforms[platform];

                // get platform path
                selectedPlatform.Path = FindPlatformPath(selectedPlatform.RegistryKey, selectedPlatform.RegistryValue) + selectedPlatform.AdditionalExecutablePath;

                // check if platform is already launched
                logger.Log("Checking if platform is executing");
                Process process = GetProcessByName(platforms[platform].ProcessName);
                relaunchPlatform = process != null;
                
                // kill all the other platform processes
                KillPlatformProcesses();

                logger.Log("Executing platform with custom enviroment");
                // origin doesn't need these modifications to launch with mods
                if (platform != LaunchPlatform.Origin)
                {
                    // run platfrom with the needed environmental variables
                    string modPath = $"ModData\\{App.SelectedPack}";
                    RunPlatformProcess(selectedPlatform.Path, App.FileSystem.BasePath + modPath);
                }
                else
                {
                    // run origin like normal
                    FrostyModExecutor.ExecuteProcess(selectedPlatform.Path, "");
                }

                logger.Log("Waiting for platform");
                try
                {
                    WaitForProcess(selectedPlatform.ProcessName, cancelToken, true, logger);
                }
                catch (OperationCanceledException)
                {
                }
            }
        });

        public override Action<ILogger, PluginManagerType, CancellationToken> PostLaunchAction => new Action<ILogger, PluginManagerType, CancellationToken>((ILogger logger, PluginManagerType type, CancellationToken cancelToken) =>
        {
            // only run platform launch system when PlatformLaunchingEnabled is true
            if (Config.Get("PlatformLaunchingEnabled", false, ConfigScope.Game))
            {
                LaunchPlatform platform = (LaunchPlatform)Enum.Parse(typeof(LaunchPlatform), Config.Get("Platform", "Origin", ConfigScope.Game));

                // origin doesn't need these modifications to launch with mods
                if (platform != LaunchPlatform.Origin)
                {
                    logger.Log("Waiting for game to launch");

                    // find game process to check when it closes
                    try
                    {
                        Process gameProcess = null;

                        // needed to weed false launch by steam
                        while (gameProcess == null || gameProcess.HasExited)
                            gameProcess = WaitForProcess(ProfilesLibrary.ProfileName, cancelToken, false, logger);

                        gameProcess.EnableRaisingEvents = true;
                        gameProcess.Exited += new EventHandler(OnGameProcessExited);
           
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
        });

        private void OnGameProcessExited(object sender, EventArgs e)
        {
            LaunchPlatform platform = (LaunchPlatform)Enum.Parse(typeof(LaunchPlatform), Config.Get("Platform", "Origin", ConfigScope.Game));

            // close and relaunch platform process without the environmental variables
            KillPlatformProcesses();
            if (relaunchPlatform)
                FrostyModExecutor.ExecuteProcess(selectedPlatform.Path, "");
        }

        private void RunPlatformProcess(string platformPath, string gamePath)
        {
            // add environmental variables
            Dictionary<string, string> env = new Dictionary<string, string>();
            env.Add("GAME_DATA_DIR", gamePath);

            // open platform process
            FrostyModExecutor.ExecuteProcess(platformPath, "", false, false, env);
        }

        private string FindPlatformPath(string key, string value)
        {
            using (RegistryKey lmKey = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\WOW6432Node\\{key}"))
            {
                return lmKey != null ? (string)lmKey.GetValue(value) : null;
            }
        }

        private Process GetProcessByName(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            if (processes.Length > 0)
                return processes[0];
            else
                return null;
        }

        private Process WaitForProcess(string name, CancellationToken cancelToken, bool exactMatch = false, ILogger logger = null)
        {
            Process foundProcess = null;
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();

                foreach (var process in Process.GetProcesses())
                {
                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        if (process.ProcessName.StartsWith(name, StringComparison.OrdinalIgnoreCase) && 
                            (!exactMatch || process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            foundProcess = process;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (foundProcess != null)
                {
                    logger?.Log("Waiting for " + foundProcess.ProcessName);
                    try
                    {
                        foundProcess.WaitForInputIdle();
                    }
                    catch (InvalidOperationException)
                    {
                        // not a graphic UI
                    }

                    return foundProcess;
                }
            }
        }

        private void KillPlatformProcess(LaunchPlatform platform)
        {
            Process platformProcess = GetProcessByName(platforms[platform].ProcessName);
            if (platformProcess != null)
                platformProcess.Kill();
        }

        private void KillPlatformProcesses()
        {
            KillPlatformProcess(LaunchPlatform.Origin);
            KillPlatformProcess(LaunchPlatform.Steam);
            KillPlatformProcess(LaunchPlatform.EpicGamesLauncher);
            KillPlatformProcess(LaunchPlatform.EADesktop);
        }
    }
}
