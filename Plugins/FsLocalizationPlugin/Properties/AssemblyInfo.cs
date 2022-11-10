using FsLocalizationPlugin;
using Frosty.Core.Attributes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using FrostySdk;

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

[assembly: PluginDisplayName("FsLocalization Loader")]
[assembly: PluginAuthor("GalaxyMan2015 & Mophead")]
[assembly: PluginVersion("1.0.1.1")]

[assembly: PluginNotValidForProfile((int)ProfileVersion.DragonAgeInquisition)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.MassEffectAndromeda)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Anthem)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Fifa17)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Fifa18)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Fifa19)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Fifa20)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Fifa21)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Fifa22)]
[assembly: PluginNotValidForProfile((int)ProfileVersion.Fifa23)]

[assembly: RegisterCustomHandler(CustomHandlerType.Ebx, typeof(FsLocalizationCustomActionHandler), ebxType: "UITextDatabase")]
[assembly: RegisterLocalizedStringDatabase(typeof(FsLocalizationStringDatabase))]

