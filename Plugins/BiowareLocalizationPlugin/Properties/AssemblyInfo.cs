using BiowareLocalizationPlugin;
using Frosty.Core.Attributes;
using FrostySdk;
using FrostySdk.Managers.Entries;
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

[assembly: PluginDisplayName("Bioware Localization Plugin")]
[assembly: PluginAuthor("GalaxyMan2015 and KrrKs")]
[assembly: PluginVersion("1.1.1.0")]
[assembly: PluginValidForProfile((int)ProfileVersion.DragonAgeInquisition)]
[assembly: PluginValidForProfile((int)ProfileVersion.MassEffectAndromeda)]
[assembly: PluginValidForProfile((int)ProfileVersion.Anthem)]


[assembly: RegisterLocalizedStringDatabase(typeof(BiowareLocalizedStringDatabase))]

[assembly: RegisterMenuExtension(typeof(BioWareLocalizedStringEditorMenuExtension))]

[assembly: RegisterCustomHandler(CustomHandlerType.Res, typeof(BiowareLocalizationCustomActionHandler), resType: ResourceType.LocalizedStringResource, ebxType: "")]

[assembly: RegisterOptionsExtension(typeof(BiowareLocalizationPluginOptions), Frosty.Core.PluginManagerType.Both)]
