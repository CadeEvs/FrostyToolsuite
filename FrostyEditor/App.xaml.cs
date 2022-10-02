﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using FrostySdk.Managers;
using FrostySdk;
using FrostyEditor.Windows;
using Frosty.Controls;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Managers;
using FrostyCore;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using FrostySdk.Managers.Entries;

namespace FrostyEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AssetManager AssetManager { get => Frosty.Core.App.AssetManager; set => Frosty.Core.App.AssetManager = value; }
        public static ResourceManager ResourceManager { get => Frosty.Core.App.ResourceManager; set => Frosty.Core.App.ResourceManager = value; }
        public static FileSystemManager FileSystem { get => Frosty.Core.App.FileSystemManager; set => Frosty.Core.App.FileSystemManager = value; }
        public static PluginManager PluginManager { get => Frosty.Core.App.PluginManager; set => Frosty.Core.App.PluginManager = value; }
        public static NotificationManager NotificationManager { get => Frosty.Core.App.NotificationManager; set => Frosty.Core.App.NotificationManager = value; }

        public static EbxAssetEntry SelectedAsset { get => Frosty.Core.App.SelectedAsset; set => Frosty.Core.App.SelectedAsset = value; }
        public static ILogger Logger { get => Frosty.Core.App.Logger; set => Frosty.Core.App.Logger = value; }

        public static string SelectedPack { get => Frosty.Core.App.SelectedPack; set => Frosty.Core.App.SelectedPack = value; }

        public static long StartTime;

        public static bool OpenProject {
            get => m_openProject;
            set => m_openProject = value;
        }

        public static string LaunchArgs { get; private set; }

        private static bool m_openProject;

        private FrostyConfiguration m_defaultConfig;

        public App()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            Frosty.Core.App.Version = entryAssembly.GetName().Version.ToString();

            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            Logger = new FrostyLogger();
            Logger.Log("Frosty Editor v{0}", Frosty.Core.App.Version);

            //FileUnblocker.UnblockDirectory(".\\");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            TypeLibrary.Initialize();
            PluginManager = new PluginManager(App.Logger, PluginManagerType.Editor);
            ProfilesLibrary.Initialize(PluginManager.Profiles);

            NotificationManager = new NotificationManager();

#if !FROSTY_DEVELOPER
            // for displaying exception box on all unhandled exceptions
            DispatcherUnhandledException += App_DispatcherUnhandledException;
#endif
            Exit += Application_Exit;

#if FROSTY_DEVELOPER
            Frosty.Core.App.Version += " (Developer)";
#elif FROSTY_ALPHA
            Frosty.Core.App.Version += $" (ALPHA {Frosty.Core.App.Version})";
#elif FROSTY_BETA
            Frosty.Core.App.Version += $" (BETA {Frosty.Core.App.Version})";
#endif
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (MainWindow is MainWindow win)
            {
                FrostyProject project = win.Project;
                if (project.IsDirty)
                {
                    string name = project.DisplayName.Replace(".fbproject", "");
                    DateTime timeStamp = DateTime.Now;

                    project.Filename = "Autosave/" + name + "_" + timeStamp.Day.ToString("D2") + timeStamp.Month.ToString("D2") + timeStamp.Year.ToString("D4") + "_" + timeStamp.Hour.ToString("D2") + timeStamp.Minute.ToString("D2") + timeStamp.Second.ToString("D2") + ".fbproject";
                    project.Save();
                }
            }

            Exception exp = e.Exception;
            using (NativeWriter writer = new NativeWriter(new FileStream("crashlog.txt", FileMode.Create)))
                writer.WriteLine($"{exp.Message}\r\n\r\n{exp.StackTrace}");

            FrostyExceptionBox.Show(exp, "Frosty Editor");
            Environment.Exit(0);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllname = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name;
            if (dllname.StartsWith("SharpDX") || dllname.StartsWith("Newtonsoft"))
            {
                FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
                return Assembly.LoadFile(fi.DirectoryName + "/ThirdParty/" + dllname + ".dll");
            }
            
            if (dllname.Equals("EbxClasses"))
            {
                FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
                return Assembly.LoadFile(fi.DirectoryName + "/Profiles/" + ProfilesLibrary.SDKFilename + ".dll");
            }
            
            if (dllname.Equals("AssetBankClasses"))
            {
                FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
                return Assembly.LoadFile(fi.DirectoryName + "/AssetBankProfiles/" + ProfilesLibrary.SDKFilename + ".dll");
            }
            
            if (PluginManager != null)
            {
                if (PluginManager.IsThirdPartyDll(dllname))
                {
                    FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
                    return Assembly.LoadFile(fi.DirectoryName + "/ThirdParty/" + dllname + ".dll");
                }

                return PluginManager.GetPluginAssembly(dllname);
            }

            return null;
        }

        public static void UpdateDiscordRpc(string state)
        {
            if (!Config.Get<bool>("DiscordRPCEnabled", false))
                return;

            DiscordRichPresence discordPresence = new DiscordRichPresence
            {
                details = ProfilesLibrary.DisplayName?.Replace("™", ""),
                state = state,
                startTimestamp = App.StartTime,
                largeImageKey = "frostylogobig",
                largeImageText = "Frosty Editor v" + Frosty.Core.App.Version.Replace(" (Developer)", "")
            };

            if (Current.MainWindow is MainWindow)
            {
                if (ProfilesLibrary.EnableExecution)
                {
                    discordPresence.smallImageKey = "frostyprojectsmall";
                    discordPresence.smallImageText = (App.Current.MainWindow as MainWindow)?.Project.DisplayName.Replace(".fbproject", "");
                }
                else
                {
                    discordPresence.smallImageKey = "frostyreadonlysmall";
                    discordPresence.smallImageText = "Read-Only Profile";
                }
            }

            DiscordRPC.Discord_UpdatePresence(ref discordPresence);
        }

        public static void InitDiscordRpc()
        {
            if (Config.Get<bool>("DiscordRPCEnabled", false))
            {
                DiscordEventHandlers handlers = new DiscordEventHandlers
                {
                    ready = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ReadyCallback>(DiscordReady),
                    disconnected = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<DisconnectedCallback>(DiscordDisconnected),
                    errored = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<ErroredCallback>(DiscordErrored),
                    joinGame = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<JoinGameCallback>(DiscordJoinGame),
                    spectateGame = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<SpectateGameCallback>(DiscordSpectateGame),
                    joinRequest = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate<JoinRequestCallback>(DiscordJoinRequest)
                };

                StartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                DiscordRPC.Discord_Initialize("478035914132815883", ref handlers, 1);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!File.Exists($"{Frosty.Core.App.GlobalSettingsPath}/editor_config.json"))
            {
                Config.UpgradeConfigs();
            }

            Config.Load();

            if (Config.Get<bool>("UpdateCheck", true) || Config.Get<bool>("UpdateCheckPrerelease", false))
            {
                CheckVersion();
            }

            // get startup profile (if one exists)
            if (Config.Get<bool>("UseDefaultProfile", false))
            {
                string prof = Config.Get<string>("DefaultProfile", null);
                if (!string.IsNullOrEmpty(prof))
                {
                    m_defaultConfig = new FrostyConfiguration(prof);
                }
                else
                {
                    Config.Add("UseDefaultProfile", false);
                    Config.Save();
                }
            }

            //check args to see if it is loading a project
            if (e.Args.Length > 0) {
                string arg = e.Args[0];
                
                if (arg.Contains(".fbproject"))
                {
                    m_openProject = true;
                    LaunchArgs = arg;
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // disable DiscordRPC
            if (Config.Current != null && Config.Get<bool>("DiscordRPCEnabled", false))
            {
                DiscordRPC.Discord_Shutdown();
            }
        }

        private void CheckVersion()
        {
            bool checkPrerelease = Config.Get<bool>("UpdateCheckPrerelease", false);
            Version localVersion = Assembly.GetEntryAssembly().GetName().Version;

            try
            {
                if (UpdateCheckerUtils.CheckVersion(checkPrerelease, localVersion))
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        MessageBoxResult mbResult = FrostyMessageBox.Show("You are using an outdated version of Frosty." + Environment.NewLine + "Would you like to download the latest version?", "Frosty Editor", MessageBoxButton.YesNo);
                        if (mbResult == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start("https://github.com/CadeEvs/FrostyToolsuite/releases/latest");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                System.Threading.Tasks.Task.Run(() => {
                    FrostyMessageBox.Show("Frosty Update Checker returned with an error:" + Environment.NewLine + e.Message, "Frosty Editor", MessageBoxButton.OK);
                });
            }
        }

        private static void DiscordReady(DiscordUser user)
        {
            //throw new NotImplementedException();
        }

        private static void DiscordDisconnected(int errorCode, string message)
        {
            //throw new NotImplementedException();
        }

        private static void DiscordErrored(int errorCode, string message)
        {
            //throw new NotImplementedException();
        }

        private static void DiscordJoinGame(string joinSecret)
        {
            //throw new NotImplementedException();
        }

        private static void DiscordSpectateGame(string spectateSecret)
        {
            //throw new NotImplementedException();
        }

        private static void DiscordJoinRequest(DiscordUser request)
        {
            //throw new NotImplementedException();
        }
    }
}
