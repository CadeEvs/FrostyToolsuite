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
using FrostyCore;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Net.Cache;
using System.Linq;

namespace FrostyEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AssetManager AssetManager { get => Frosty.Core.App.AssetManager; set => Frosty.Core.App.AssetManager = value; }
        public static ResourceManager ResourceManager { get => Frosty.Core.App.ResourceManager; set => Frosty.Core.App.ResourceManager = value; }
        public static FileSystem FileSystem { get => Frosty.Core.App.FileSystem; set => Frosty.Core.App.FileSystem = value; }
        public static PluginManager PluginManager { get => Frosty.Core.App.PluginManager; set => Frosty.Core.App.PluginManager = value; }
        public static EbxAssetEntry SelectedAsset { get => Frosty.Core.App.SelectedAsset; set => Frosty.Core.App.SelectedAsset = value; }
        public static ILogger Logger { get => Frosty.Core.App.Logger; set => Frosty.Core.App.Logger = value; }

        public static string SelectedPack { get => Frosty.Core.App.SelectedPack; set => Frosty.Core.App.SelectedPack = value; }

        public static string Version = "";
        public static long StartTime;

        public static bool OpenProject {
            get => openProject;
            set => openProject = value;
        }

        public static string LaunchArgs { get; private set; }

        private static bool openProject;

        private FrostyConfiguration defaultConfig;

        public App()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            Version = entryAssembly.GetName().Version.ToString();

            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            Logger = new FrostyLogger();
            Logger.Log("Frosty Editor v{0}", Version);

            FileUnblocker.UnblockDirectory(".\\");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            TypeLibrary.Initialize();
            PluginManager = new PluginManager(App.Logger, PluginManagerType.Editor);
            ProfilesLibrary.Initialize(PluginManager.Profiles);

            // for displaying exception box on all unhandled exceptions
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Exit += Application_Exit;

            string test = FrostyEditor.Properties.Resources.BuildDate;
            test = test.Substring(test.IndexOf(' ') + 1);
            test = test.Substring(0, test.IndexOf(' '));

#if FROSTY_DEVELOPER
            Version += " (Developer)";
#elif FROSTY_ALPHA
            Version += $" (ALPHA {Frosty.Core.App.Version})";
#elif FROSTY_BETA
            Version += $" (BETA {Frosty.Core.App.Version})";
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

        public static void UpdateDiscordRPC(string state)
        {
            if (!Config.Get<bool>("DiscordRPCEnabled", false))
                return;

            DiscordRichPresence discordPresence = new DiscordRichPresence
            {
                details = ProfilesLibrary.DisplayName.Replace("™", ""),
                state = state,
                startTimestamp = App.StartTime,
                largeImageKey = "frostylogobig",
                largeImageText = "Frosty Editor v" + App.Version.Replace(" (Developer)", "")
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

        public static void InitDiscordRPC()
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
                Config.UpgradeConfigs();

            Config.Load();

            if (Config.Get<bool>("UpdateCheck", true) || Config.Get<bool>("UpdateCheckPrerelease", true))
                checkVersion();

            // get startup profile (if one exists)
            if (Config.Get<bool>("UseDefaultProfile", false))
            {
                string prof = Config.Get<string>("DefaultProfile", null);
                if (!string.IsNullOrEmpty(prof))
                    defaultConfig = new FrostyConfiguration(prof);
                else
                {
                    Config.Add("UseDefaultProfile", false);
                    Config.Save();
                }
            }

            //check args to see if it is loading a project
            if (e.Args.Length > 0) {
                string arg = e.Args[0];
                if (arg.Contains(".fbproject")) {
                    openProject = true;
                    LaunchArgs = arg;

                    //get game profile from project file
                    using (NativeReader reader = new NativeReader(new FileStream(arg, FileMode.Open, FileAccess.Read))) {
                        if (reader.ReadULong() == 0x00005954534F5246) {
                            reader.ReadUInt();
                            string gameProfile = reader.ReadNullTerminatedString();
                            try { 
                                defaultConfig = new FrostyConfiguration(gameProfile); 
                            }
                            catch { 
                                FrostyMessageBox.Show("There was an error when trying to load project using the profile: " + gameProfile, "Frosty Editor");
                            }
                        }
                    }
                }
            }

            if (defaultConfig != null)
            {
                // load profile
                if (!ProfilesLibrary.Initialize(defaultConfig.ProfileName))
                {
                    FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Editor");
                    return;
                }

                this.StartupUri = new System.Uri("/FrostyEditor;component/Windows/SplashWindow.xaml", System.UriKind.Relative);

                App.InitDiscordRPC();
                App.UpdateDiscordRPC("Loading...");
            }
        }

        public void checkVersion() {
            bool updateCheckPrerelease = Config.Get<bool>("UpdateCheckPrerelease", true);
            try {
                using (var client = new WebClient()) {
                    client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    client.Headers.Add(HttpRequestHeader.UserAgent, "request");

                    dynamic results = JsonConvert.DeserializeObject<dynamic>(client.DownloadString("https://api.github.com/repos/CadeEvs/FrostyToolsuite/releases"));
                    if (!updateCheckPrerelease) results = JsonConvert.DeserializeObject<dynamic>(client.DownloadString("https://api.github.com/repos/CadeEvs/FrostyToolsuite/releases/latest"));

                    string latestVersionString = results[0].tag_name;
                    if (!updateCheckPrerelease) latestVersionString = results.tag_name;
                    string versionString = Assembly.GetEntryAssembly().GetName().Version.ToString();

                    // alpha/beta number
                    int prereleaseLocalVersion = Frosty.Core.App.Version;
                    int prereleaseLatestVersion = 0;

                    // 0 is release, 1 is beta, 2 is alpha
                    int releaseLocalType = 0;
                    int releaseLatestType = 0;

#if FROSTY_ALPHA
            releaseLocalType = 2;
#elif FROSTY_BETA
            releaseLocalType = 1;
#endif

                    if (latestVersionString.Contains("beta")) releaseLatestType = 1;
                    if (latestVersionString.Contains("alpha")) releaseLatestType = 2;
                    if ((bool)results[0].prerelease) {
                        prereleaseLatestVersion = int.Parse(latestVersionString.Last().ToString());
                        latestVersionString = latestVersionString.Substring(0, latestVersionString.IndexOf("-"));
                    }
                    
                    var latestVersion = new Version(latestVersionString.Substring(1));
                    var version = new Version(versionString);

                    if (latestVersion.MinorRevision < 0) latestVersion = new Version(latestVersion.ToString() + ".0");

                    var result = version.CompareTo(latestVersion);
                    if (result <= 0) {
                        if ((releaseLatestType < releaseLocalType) || (releaseLatestType == releaseLocalType && prereleaseLatestVersion > prereleaseLocalVersion)) {
                            MessageBoxResult mbResult = MessageBox.Show("You are using an outdated version of Frosty." + Environment.NewLine + "Would you like to download the latest version?", "Frosty Editor", MessageBoxButton.YesNo);
                            if (mbResult == MessageBoxResult.Yes) 
                                System.Diagnostics.Process.Start("https://github.com/CadeEvs/FrostyToolsuite/releases/latest");
                        }
                    }
                }
            }
            catch (Exception e) {
                FrostyMessageBox.Show(e.Message, "Frosty Editor", MessageBoxButton.OK);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Config.Current != null && Config.Get<bool>("DiscordRPCEnabled", false))
                DiscordRPC.Discord_Shutdown();
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
