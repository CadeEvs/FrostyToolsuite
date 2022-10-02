using MeshSetPlugin;
using Frosty.Core.Attributes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;
using MeshSetPlugin.Handlers;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

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

#if FROSTY_DEVELOPER
[assembly: XmlnsDefinition("FrostyDeveloper", "FrostyEditor.Controls")]
#endif

[assembly: PluginDisplayName("MeshSet Editor")]
[assembly: PluginAuthor("GalaxyMan2015")]
[assembly: PluginVersion("1.0.1.0")]

[assembly: RegisterOptionsExtension(typeof(MeshOptions))]
[assembly: RegisterAssetDefinition("RigidMeshAsset", typeof(RigidMeshAssetDefinition))]
[assembly: RegisterAssetDefinition("SkinnedMeshAsset", typeof(SkinnedMeshAssetDefinition))]
[assembly: RegisterAssetDefinition("CompositeMeshAsset", typeof(CompositeMeshAssetDefinition))]

[assembly: RegisterCustomHandler(CustomHandlerType.Res, typeof(ShaderBlockDepotCustomActionHandler), resType: ResourceType.ShaderBlockDepot)]