using Frosty.Core.Interfaces;
using Frosty.Core.Managers;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System;
using System.Windows;
using Frosty.Controls;
using Frosty.Core.Windows;
using System.Collections.Generic;
using FrostySdk.Managers.Entries;

namespace Frosty.Core
{
    public sealed class App
    {
        // managers
        public static AssetManager AssetManager;
        public static ResourceManager ResourceManager;
        public static FileSystemManager FileSystemManager;
        public static PluginManager PluginManager;
        public static NotificationManager NotificationManager;

        public static List<int> WhitelistedBundles = new List<int>();

        public static EbxAssetEntry SelectedAsset;
        public static string SelectedProfile;
        public static string SelectedPack;
        public static ILogger Logger;

        public static bool IsEditor = true;

        public static string Version = "";
        public static readonly int MinorVersion = 1;

        public static string ProfileSettingsPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Frosty/" + ProfilesLibrary.ProfileName;
        public static string GlobalSettingsPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Frosty";

        public static IEditorWindow EditorWindow => Application.Current.MainWindow as IEditorWindow;

        public static void ClearProfileData()
        {
            // clear out all global managers
            AssetManager = null;
            ResourceManager = null;
            FileSystemManager = null;

            PluginManager.Clear();
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        public static bool LoadProfile(string profile)
        {
            // load profiles
            if (!ProfilesLibrary.Initialize(profile))
            {
                FrostyMessageBox.Show("There was an error when trying to load game using specified profile.", "Frosty Core");
                return false;
            }

            if (!IsEditor && !ProfilesLibrary.EnableExecution)
            {
                FrostyMessageBox.Show("The selected profile is a read-only profile, and therefore cannot be loaded in the mod manager", "Frosty Mod Manager");
                return false;
            }
            
            // open profile task window
            return FrostyProfileTaskWindow.Show(Application.Current.MainWindow);
        }
    }
}
