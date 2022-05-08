using Frosty.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using FrostyCore;
using FrostyEditor;
using FrostyModManager.Windows;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Windows;

namespace FrostyModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ILogger Logger { get => Frosty.Core.App.Logger; set => Frosty.Core.App.Logger = value; }
        
        public static string SelectedPack { get => Frosty.Core.App.SelectedPack; set => Frosty.Core.App.SelectedPack = value; }

        public static string Version = "";

        public static bool LaunchGameImmediately 
        { 
            get => launchGameImmediately;
            set => launchGameImmediately = value;
        }

        public static string LaunchProfile { get; private set; }
        public static string LaunchArgs { get; private set; }

        public static PluginManager PluginManager { get => Frosty.Core.App.PluginManager; set => Frosty.Core.App.PluginManager = value; }

        private List<FrostyConfiguration> configs = new List<FrostyConfiguration>();
        private FrostyConfiguration defaultConfig = null;

        private Config ini = new Config();

        private static bool launchGameImmediately;

        public App()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            Version = entryAssembly.GetName().Version.ToString();

            Logger = new FrostyLogger();
            Logger.Log("Frosty Mod Manager v{0}", Version);

            FileUnblocker.UnblockDirectory(".\\");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            TypeLibrary.Initialize();
            PluginManager = new PluginManager(App.Logger, PluginManagerType.ModManager);
            ProfilesLibrary.Initialize(PluginManager.Profiles);

            // for displaying exception box on all unhandled exceptions
            DispatcherUnhandledException += App_DispatcherUnhandledException;

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
            Exception exp = e.Exception;

            using (NativeWriter writer = new NativeWriter(new FileStream("crashlog.txt", FileMode.Create)))
                writer.WriteLine($"{exp.Message}\r\n\r\n{exp.StackTrace}");

            FrostyExceptionBox.Show(exp, "Frosty Mod Manager");
            Environment.Exit(0);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllname = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name;
            if (dllname.StartsWith("SharpDX") || dllname.StartsWith("Newtonsoft"))
            {
                FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
                return Assembly.LoadFile(fi.DirectoryName + "/ThirdParty/" + dllname + ".dll");
            }
            else if (dllname.Equals("EbxClasses"))
            {
                FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
                return Assembly.LoadFile(fi.DirectoryName + "/Profiles/" + ProfilesLibrary.SDKFilename + ".dll");
            }
            else if (PluginManager != null)
            {
                return PluginManager.GetPluginAssembly(dllname);
            }

            return null;
        }

        //private void RefreshConfigurationList()
        //{
        //    configs.Clear();
        //    foreach (string s in Directory.EnumerateFiles("./", "FrostyModManager*.ini"))
        //    {
        //        try
        //        {
        //            FrostyConfiguration config = new FrostyConfiguration(s);
        //            configs.Add(config);
        //        }
        //        catch (Exception /*ex*/)
        //        {
        //            //FrostyMessageBox.Show("Couldn't load profile from '" + s + "': \n\n" + ex.ToString());
        //        }
        //    }
        //}

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!File.Exists($"{Frosty.Core.App.GlobalSettingsPath}/manager_config.json"))
                Config.UpgradeConfigs();

            //RefreshConfigurationList();

            Config.Load();
            //ini.LoadEntries("DefaultSettings.ini");

            if (Config.Get<bool>("UpdateCheck", true) || Config.Get<bool>("UpdateCheckPrerelease", true))
                checkVersion();

            //string defaultConfigname = ini.GetEntry("Init", "DefaultConfiguration", "");

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
            //foreach (FrostyConfiguration name in configs)
            //{
            //    if (name.ProfileName == defaultConfigname)
            //    {
            //        defaultConfig = name;
            //    }
            //}

            // Launches the Frosty Mod Manager is there is a Default Config
            if (defaultConfig != null)
            {
                // load profile
                if (!ProfilesLibrary.Initialize(defaultConfig.ProfileName))
                {
                    FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Mod Manager");
                    return;
                }

                StartupUri = new Uri("/FrostyModManager;component/Windows/SplashWindow.xaml", System.UriKind.Relative);
            }
            //if (defaultConfig != null)
            //{
            //    App.configFilename = defaultConfig.Filename;
            //    Config.Load(defaultConfig.Config); // Load game config

            //    // load profiles
            //    if (!ProfilesLibrary.Initialize(Config.Get<string>("Init", "Profile", "")))
            //    {
            //        FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Editor");
            //        return;
            //    }

            //    this.StartupUri = new Uri("/FrostyModManager;component/Windows/MainWindow.xaml", UriKind.Relative);
            //}

            StringBuilder sb = new StringBuilder();
            if (e.Args.Length > 0)
            {
                string arg = e.Args[0];
                if (arg == "-launch")
                {
                    if (e.Args.Length < 2)
                    {
                        FrostyMessageBox.Show("-launch argument found, but missing profile name", "Frosty Mod Manager");
                        Current.Shutdown();
                        return;
                    }

                    launchGameImmediately = true;
                    LaunchProfile = e.Args[1];

                    for (int i = 2; i < e.Args.Length; i++)
                    {
                        arg = e.Args[i];
                        sb.Append(arg + " ");
                    }
                }
            }

            LaunchArgs = sb.ToString().Trim();
        }

        public void checkVersion() {
            bool updateCheckPrerelease = Config.Get<bool>("UpdateCheckPrerelease", true);
            try {
                using (var client = new WebClient()) {
                    client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    client.Headers.Add(HttpRequestHeader.UserAgent, "request");

                    dynamic results;
                    if (updateCheckPrerelease) results = JsonConvert.DeserializeObject<dynamic>(client.DownloadString("https://api.github.com/repos/CadeEvs/FrostyToolsuite/releases"));
                    else results = JsonConvert.DeserializeObject<dynamic>(client.DownloadString("https://api.github.com/repos/CadeEvs/FrostyToolsuite/releases/latest"));

                    string latestVersionString;
                    if (updateCheckPrerelease) latestVersionString = results[0].tag_name;
                    else latestVersionString = results.tag_name;

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
                        if ((releaseLatestType < releaseLocalType) || (releaseLatestType == releaseLocalType && prereleaseLatestVersion > prereleaseLocalVersion) || (releaseLatestType == 0 && releaseLocalType == 0)) {
                            System.Threading.Tasks.Task.Run(() => {
                                MessageBoxResult mbResult = FrostyMessageBox.Show("You are using an outdated version of Frosty." + Environment.NewLine + "Would you like to download the latest version?", "Frosty Mod Manager", MessageBoxButton.YesNo);
                                if (mbResult == MessageBoxResult.Yes)
                                    System.Diagnostics.Process.Start("https://github.com/CadeEvs/FrostyToolsuite/releases/latest");
                            });
                        }
                    }
                }
            }
            catch (Exception e) {
                System.Threading.Tasks.Task.Run(() => {
                    FrostyMessageBox.Show("Frosty Update Checker returned with an error:" + Environment.NewLine + e.Message, "Frosty Mod Manager", MessageBoxButton.OK);
                });
            }
        }
    }
}
