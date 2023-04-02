using BundleManager;
using Frosty.Core.Attributes;
using FrostySdk;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

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

[assembly: PluginDisplayName("Bundle Manager")]
[assembly: PluginAuthor("Mophead")]
[assembly: PluginVersion("1.0.0.0")]

//[assembly: PluginValidForProfile((int)ProfileVersion.Battlefield1)]
//[assembly: PluginValidForProfile((int)ProfileVersion.Battlefield5)]
//[assembly: PluginValidForProfile((int)ProfileVersion.StarWarsBattlefront)]
[assembly: PluginValidForProfile((int)ProfileVersion.StarWarsBattlefrontII)]
[assembly: RegisterOptionsExtension(typeof(BundleManagerOptionsMenu.BundleManagerOptions))]
[assembly: RegisterMenuExtension(typeof(MenuExtensions.BundleManagerMenuExtension))]
[assembly: RegisterMenuExtension(typeof(MenuExtensions.BundleManagerLevelsMenuExtension))]
[assembly: RegisterMenuExtension(typeof(MenuExtensions.ClearBundlesMenuExtension))]
[assembly: RegisterMenuExtension(typeof(MenuExtensions.ExportPrerequisitsMenuExtension))]

