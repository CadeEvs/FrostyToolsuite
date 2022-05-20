using Frosty.Core.Attributes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using TestPlugin.Definitions;
using TestPlugin.EditorExecutions;
using TestPlugin.Extensions;
using TestPlugin.Handlers;
using TestPlugin.Managers;
using TestPlugin.TypeOverrides;

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page, 
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4b612468-9b6a-4304-88a5-055c3575eb3d")]

// This is the heart and soul of a plugin. Everything you want exposed to the plugin
// system will be entered here via assembly attributes.

// This first section describes the plugin, where you can provide details on the 
// name, author and version of the plugin.

[assembly: PluginDisplayName("Example Plugin")]
[assembly: PluginAuthor("GalaxyMan2015")]
[assembly: PluginVersion("1.0.1.0")]

// ValidFor and NotValidFor attributes can control for which profiles a particular plugin
// will load for.

// Indicates that the plugin will load only if the Dragon Age Inquisition profile is loaded
//[assembly: PluginValidForProfile("DragonAgeInquisition")]

// Indicates that the plugin will not load if the Anthem profile is loaded
//[assembly: PluginNotValidForProfile("Anthem")]

// Profile Names:
//
// bf4 = Battlefield 4
// bfh = Battlefield Hardline
// NSF14 = Need for Speed Rivals
// DragonAgeInquisition = Dragon Age Inquisition
// NFS16 = Need for Speed
// starwarsbattlefront = Star Wars Battlefront
// PVZ.Main_Win64_Retail = Plants vs Zombies Garden Warfare
// GW2.Main_Win64_Retail = Plants vs Zombies Garden Warfare 2
// MirrorsEdgeCatalyst = Mirrors Edge Catalyst
// FIFA17 = FIFA 17
// bf1 = Battlefield 1
// MassEffectAndromeda = Mass Effect Andromeda
// FIFA18 = FIFA 18
// NeedForSpeedPayback = Need for Speed Payback
// starwarsbattlefrontii = Star Wars Battlefront 2
// Madden19 = Madden NFL 19
// FIFA19 = FIFA 19
// bfv = Battlefield V
// NFSOL = Need for Speed Online
// Anthem = Anthem
// Madden20 = Madden NFL 20
// PVZBattleforNeighborville = Plants vs Zombies Battle for Neighborville
// FIFA20 = FIFA 20
// NeedForSpeedHeat = Need for Speed Heat
// sws = Star Wars Squadrons

// Menu extenions allow you to add your own menu items to the main menu bar of the editor.
// The type specified must inherit from the base MenuExtension class.

[assembly: RegisterMenuExtension(typeof(InitFsMenuExtension))]

// Asset definitions allow you to specify custom logic and information about specific
// asset types, these asset definitions allow you to override the icon and editors that
// will be used, as well as specify import/export logic.

//[assembly: RegisterAssetDefinition("SvgImage", typeof(SvgImageAssetDefinition))]
[assembly: RegisterAssetDefinition("DifficultyWeaponTableData", typeof(DifficultyWeaponTableDataAssetDefinition))]

// Custom handlers are a way to override the way data is handled for a specific type.
// There are different handlers for Res and Ebx types, used in conjunction with ModifiedResource
// allows you to store modified data from the editor then use that modified data to merge data together from
// different mods from the mod manager.

//[assembly: RegisterCustomHandler(CustomHandlerType.Ebx, typeof(SvgImageCustomActionHandler), ebxType: "SvgImage")]
[assembly: RegisterCustomHandler(CustomHandlerType.Ebx, typeof(DifficultyWeaponTableActionHandler), ebxType: "DifficultyWeaponTableData")]
//[assembly: RegisterCustomHandler(CustomHandlerType.Ebx, typeof(NetworkRegistryActionHandler), ebxType: "NetworkRegistryAsset")]

// Editor executions allow you to run custom code prior or after the mod applying process.
[assembly: RegisterExecutionAction(typeof(CustomExecutionAction))]

[assembly: RegisterCustomAssetManager("fs", typeof(FsFileManager))]

// Allows saving to mods for custom assets
[assembly: RegisterCustomHandler(CustomHandlerType.CustomAsset, typeof(InitFsCustomActionHandler), customType: "fs")]

[assembly: RegisterTypeOverride("UnlockDataCollection", typeof(UnlockDataCollectionTypeOverride))]
[assembly: RegisterTypeOverride("SubWorldReferenceObjectData", typeof(SubworldTypeOverride))]