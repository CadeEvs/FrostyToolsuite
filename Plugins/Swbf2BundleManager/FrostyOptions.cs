using Frosty.Core;
using Frosty.Core.Controls.Editors;
using FrostySdk.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BundleManager
{
    internal class BundleManagerOptionsMenu
    {
        #region Options Load
        public class FrostyDisplayAdapterEditor : FrostyCustomComboDataEditor<string, string>
        {
        }

        [DisplayName("Bundle Manager Options")]

        public class BundleManagerOptions : OptionsExtension
        {
            //[Category("General Options")]
            //[DisplayName("Enable Log Debug Mode")]
            //[Description("Shows a bunch of edits in the log so that causes for crash issues can be detected. Will significantly slow down the Bundle Manager.")]
            //public bool BMO_EnableDebugMode { get; set; } = false;

            [Category("General Options")]
            [DisplayName("Enable Log Export")]
            [Description("Enabling will make the Bundle Manager export a csv log of edits at the end of each BM run.")]
            public bool BMO_EnableLogExport { get; set; } = true;

            [Category("General Options")]
            [DisplayName("Enable Prerequisites")]
            [Description("Enabling will make the Bundle Manager use Prerequisites found in your Frosty Editor/Caches/BundleManagerPrerequisites file. Prerequisites allow you to make bundle managed mods be compatible")]
            public bool BMO_EnablePrerequisites { get; set; } = false;

            [Category("General Options")]
            [DisplayName("Enable Bundle Order Log Export")]
            [Description("Enabling will make the Bundle Manager export a log of the bundle manager parent order in the log export csv file.")]
            public bool BMO_EnableBundleLogExport { get; set; } = false;

            [Category("General Options")]
            [DisplayName("Complete Network Registries")]
            [Description("Disabling prevents the Bundle Manager from making bundle changes to network registries")]
            public bool BMO_CompleteNetworkRegistries { get; set; } = true;

            [Category("General Options")]
            [DisplayName("Create new Network Registries")]
            [Description("Creates new network registries rather than modifying old ones. WARNING: THIS CAN CAUSE DESYNCS IN ONLINE MATCHES, RECOMMENDED ONLY FOR TESTING OR SINGLE PLAYER MODS")]
            public bool BMO_CreateNetworkRegistries { get; set; } = false;

            [Category("General Options")]
            [DisplayName("Complete MeshVariationDBs")]
            [Description("Disabling prevents the Bundle Manager from making bundle changes to mesh variation databases")]
            public bool BMO_CompleteMeshVariationDBs { get; set; } = true;

            [Category("General Options")]
            [DisplayName("Complete Shared Bundles")]
            [Description("Disabling prevents the Bundle Manager from making bundle changes to shared bundles")]
            public bool BMO_CompleteSharedBundles { get; set; } = true;

            [Category("General Options")]
            [DisplayName("Complete Sublevel Bundles")]
            [Description("Disabling prevents the Bundle Manager from making bundle changes to sublevel bundles")]
            public bool BMO_CompleteSublevelBundles { get; set; } = true;

            [Category("General Options")]
            [DisplayName("Complete Blueprint Bundles")]
            [Description("Disabling prevents the Bundle Manager from making bundle changes to blueprint bundles")]
            public bool BMO_CompleteBlueprintBundles { get; set; } = true;

            [Category("General Options")]
            [DisplayName("Complete Added Bundles")]
            [Description("Disabling prevents the Bundle Manager from making bundle changes to newly added bundles")]
            public bool BMO_CompleteAddedBundles { get; set; } = true;

            [Category("General Options")]
            [DisplayName("Ignore TOC/Manifest Chunks")]
            [Description("Enabling prevents the Bundle Manager from making bundle changes to preexisting chunks loaded in TOC or Manifest files")]
            public bool BMO_IgnoreTocChunks { get; set; } = false;

            [Category("General Options")]
            [DisplayName("Whitelist bundles")]
            [Description("When a bundle manager enumeration is complete and you launch the game, only modifications made to the bundles you allowed the bundle manager to edit will be made. If you disabled the BM's ability to edit bpbs for example, they won't be modified when launching the game")]
            public bool BMO_WhitelistBundles { get; set; } = true;

            [Category("Swbf2 Options")]
            [DisplayName("SP Campaign bundles")]
            public bool BMO_Sublevel_SP { get; set; } = false;

            [Category("Swbf2 Options")]
            [DisplayName("MP bundles")]
            public bool BMO_Sublevel_MP { get; set; } = true;

            [Category("Swbf2 Options")]
            [DisplayName("Load Frontend Animations universally")]
            [Description("When completing a bundle enumeration, the animation files of frontend and the collection menu will get loaded across all LevelData bundles which the BM is allowed to edit")]
            public bool BMO_LoadFrontendAnimations { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Mode1 (Supremacy)")]
            public bool BMO_Sublevel_Mode1 { get; set; } = true;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Mode9 (Attack/Defend)")]
            public bool BMO_Sublevel_Mode9 { get; set; } = true;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("DeathMatch_Skirmish (Arcade)")]
            public bool BMO_Sublevel_Skirmish { get; set; } = true;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Space Arcade")]
            public bool BMO_Sublevel_SpaceArcade { get; set; } = true;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Mode3 (Ewok Hunt)")]
            public bool BMO_Sublevel_Mode3 { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Mode5 (Extraction)")]
            public bool BMO_Sublevel_Mode5 { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Mode6 (Hero Showdown)")]
            public bool BMO_Sublevel_Mode6 { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Mode7 (Hero Starfighters)")]
            public bool BMO_Sublevel_Mode7 { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("ModeC (Jetpack Cargo)")]
            public bool BMO_Sublevel_ModeC { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("DeathMatch_Online (Blast)")]
            public bool BMO_Sublevel_DeathMatchOnline { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("PlanetaryBattles (Strike)")]
            public bool BMO_Sublevel_PlanetaryBattles { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("FantasyBattles (Galactic Assault)")]
            public bool BMO_Sublevel_FantasyBattles { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("Heroes Versus Villains")]
            public bool BMO_Sublevel_HvsV { get; set; } = false;

            [Category("Swbf2 Gamemode Options")]
            [DisplayName("SpaceBattles (Starfighter Assault)")]
            public bool BMO_Sublevel_SpaceBattles { get; set; } = false;





            public override void Load()
            {
                //BMO_EnableDebugMode = Config.Get<bool>("BMO_EnableDebugMode", false);
                BMO_EnableLogExport = Config.Get<bool>("BMO_EnableLogExport", true);
                BMO_EnablePrerequisites = Config.Get<bool>("BMO_EnablePrerequisites", false);
                BMO_EnableBundleLogExport = Config.Get<bool>("BMO_EnableBundleLogExport", false);
                BMO_CompleteNetworkRegistries = Config.Get<bool>("BMO_CompleteNetworkRegistries", true);
                BMO_CreateNetworkRegistries = Config.Get<bool>("BMO_CreateNetworkRegistries", false);
                BMO_CompleteMeshVariationDBs = Config.Get<bool>("BMO_CompleteMeshVariationDBs", true);
                BMO_CompleteSharedBundles = Config.Get<bool>("BMO_CompleteSharedBundles", true);
                BMO_CompleteSublevelBundles = Config.Get<bool>("BMO_CompleteSublevelBundles", true);
                BMO_CompleteBlueprintBundles = Config.Get<bool>("BMO_CompleteBlueprintBundles", true);
                BMO_CompleteAddedBundles = Config.Get<bool>("BMO_CompleteAddedBundles", true);
                BMO_WhitelistBundles = Config.Get<bool>("BMO_WhitelistBundles", true);
                BMO_IgnoreTocChunks = Config.Get<bool>("BMO_IgnoreTocChunks", false);
                BMO_Sublevel_SP = Config.Get<bool>("BMO_Sublevel_SP", false);
                BMO_Sublevel_MP = Config.Get<bool>("BMO_Sublevel_MP", true);
                BMO_LoadFrontendAnimations = Config.Get<bool>("BMO_LoadFrontendAnimations", false);
                BMO_Sublevel_Mode1 = Config.Get<bool>("BMO_Sublevel_Mode1", true);
                BMO_Sublevel_Mode9 = Config.Get<bool>("BMO_Sublevel_Mode9", true);
                BMO_Sublevel_Skirmish = Config.Get<bool>("BMO_Sublevel_Skirmish", true);
                BMO_Sublevel_SpaceArcade = Config.Get<bool>("BMO_Sublevel_SpaceArcade", true);
                BMO_Sublevel_Mode3 = Config.Get<bool>("BMO_Sublevel_Mode3", false);
                BMO_Sublevel_Mode5 = Config.Get<bool>("BMO_Sublevel_Mode5", false);
                BMO_Sublevel_Mode6 = Config.Get<bool>("BMO_Sublevel_Mode6", false);
                BMO_Sublevel_Mode7 = Config.Get<bool>("BMO_Sublevel_Mode7", false);
                BMO_Sublevel_ModeC = Config.Get<bool>("BMO_Sublevel_ModeC", false);
                BMO_Sublevel_DeathMatchOnline = Config.Get<bool>("BMO_Sublevel_DeathMatchOnline", false);
                BMO_Sublevel_PlanetaryBattles = Config.Get<bool>("BMO_Sublevel_PlanetaryBattles", false);
                BMO_Sublevel_FantasyBattles = Config.Get<bool>("BMO_Sublevel_FantasyBattles", false);
                BMO_Sublevel_HvsV = Config.Get<bool>("BMO_Sublevel_HvsV", false);
                BMO_Sublevel_SpaceBattles = Config.Get<bool>("BMO_Sublevel_SpaceBattles", false);
            }

            public override void Save()
            {
                //Config.Add("BMO_EnableDebugMode", BMO_EnableDebugMode);
                Config.Add("BMO_EnableLogExport", BMO_EnableLogExport);
                Config.Add("BMO_EnablePrerequisites", BMO_EnablePrerequisites);
                Config.Add("BMO_EnableBundleLogExport", BMO_EnableBundleLogExport);
                Config.Add("BMO_CompleteNetworkRegistries", BMO_CompleteNetworkRegistries);
                Config.Add("BMO_CreateNetworkRegistries", BMO_CreateNetworkRegistries);
                Config.Add("BMO_CompleteMeshVariationDBs", BMO_CompleteMeshVariationDBs);
                Config.Add("BMO_CompleteSharedBundles", BMO_CompleteSharedBundles);
                Config.Add("BMO_CompleteSublevelBundles", BMO_CompleteSublevelBundles);
                Config.Add("BMO_CompleteBlueprintBundles", BMO_CompleteBlueprintBundles);
                Config.Add("BMO_CompleteAddedBundles", BMO_CompleteAddedBundles);
                Config.Add("BMO_WhitelistBundles", BMO_WhitelistBundles);
                Config.Add("BMO_IgnoreTocChunks", BMO_IgnoreTocChunks);
                Config.Add("BMO_Sublevel_SP", BMO_Sublevel_SP);
                Config.Add("BMO_Sublevel_MP", BMO_Sublevel_MP);
                Config.Add("BMO_LoadFrontendAnimations", BMO_LoadFrontendAnimations);
                Config.Add("BMO_Sublevel_Mode1", BMO_Sublevel_Mode1);
                Config.Add("BMO_Sublevel_Mode9", BMO_Sublevel_Mode9);
                Config.Add("BMO_Sublevel_Skirmish", BMO_Sublevel_Skirmish);
                Config.Add("BMO_Sublevel_SpaceArcade", BMO_Sublevel_SpaceArcade);
                Config.Add("BMO_Sublevel_Mode3", BMO_Sublevel_Mode3);
                Config.Add("BMO_Sublevel_Mode5", BMO_Sublevel_Mode5);
                Config.Add("BMO_Sublevel_Mode6", BMO_Sublevel_Mode6);
                Config.Add("BMO_Sublevel_Mode7", BMO_Sublevel_Mode7);
                Config.Add("BMO_Sublevel_ModeC", BMO_Sublevel_ModeC);
                Config.Add("BMO_Sublevel_DeathMatchOnline", BMO_Sublevel_DeathMatchOnline);
                Config.Add("BMO_Sublevel_PlanetaryBattles", BMO_Sublevel_PlanetaryBattles);
                Config.Add("BMO_Sublevel_FantasyBattles", BMO_Sublevel_FantasyBattles);
                Config.Add("BMO_Sublevel_HvsV", BMO_Sublevel_HvsV);
                Config.Add("BMO_Sublevel_SpaceBattles", BMO_Sublevel_SpaceBattles);
            }
        }

        #endregion
    }
}
