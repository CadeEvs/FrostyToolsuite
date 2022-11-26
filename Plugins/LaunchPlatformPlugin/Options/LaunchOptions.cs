using Frosty.Core;
using Frosty.Core.Controls.Editors;
using Frosty.Core.Misc;
using FrostySdk.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LaunchPlatformPlugin.Options
{
    public enum LaunchPlatform
    {
        Origin,
        EADesktop,
        Steam,
        EpicGamesLauncher
    }

    public class FrostyPlatformDataEditor : FrostyCustomComboDataEditor<string, string>
    {
    }

    [DisplayName("Launch Options")]
    public class LaunchOptions : OptionsExtension
    {
        [Category("General")]
        [Description("Enables the platform launching system. This is not needed if only Origin is being used.")]
        [DisplayName("Platform Launching Enabled")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        public bool PlatformLaunchingEnabled { get; set; } = false;

        [Category("General")]
        [Description("Selects the specific platform Frosty should launch the game on.")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Struct)]
        [Editor(typeof(FrostyPlatformDataEditor))]
        [DependsOn("PlatformLaunchingEnabled")]
        public CustomComboData<string, string> Platform { get; set; }

        [Category("Exit Settings")]
        [DisplayName("Keep Platform Launched")]
        [Description("Keeps the platform launched after exiting the game even if it was not launched before.")]
        [EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
        [DependsOn("PlatformLaunchingEnabled")]
        public bool ShouldRelaunchPlatform { get; set; } = false;

        public override void Load()
        {
            List<string> platforms = Enum.GetNames(typeof(LaunchPlatform)).ToList();
            Platform = new CustomComboData<string, string>(platforms, platforms) { SelectedIndex = platforms.IndexOf(Config.Get<string>("Platform", "Origin", ConfigScope.Game)) };

            ShouldRelaunchPlatform = Config.Get("ShouldRelaunchPlatform", false, ConfigScope.Game);
            PlatformLaunchingEnabled = Config.Get("PlatformLaunchingEnabled", false, ConfigScope.Game);
        }

        public override void Save()
        {
            Config.Add("Platform", Platform.SelectedName, ConfigScope.Game);
            Config.Add("ShouldRelaunchPlatform", ShouldRelaunchPlatform, ConfigScope.Game);
            Config.Add("PlatformLaunchingEnabled", PlatformLaunchingEnabled, ConfigScope.Game);
        }
    }
}
