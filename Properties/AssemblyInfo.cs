using Frosty.Core.Attributes;
using FrostySdk.Managers;
using LevelEditorPlugin.Definitions;
using LevelEditorPlugin.Extensions;
using LevelEditorPlugin.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
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

[assembly: PluginDisplayName("LevelEditor")]
[assembly: PluginAuthor("GalaxyMan2015 and Cade")]
[assembly: PluginVersion("1.0.0.0")]

[assembly: RegisterMenuExtension(typeof(EntityGeneratorExtension))]
[assembly: RegisterMenuExtension(typeof(DumpLayoutsToLogExtension))]
[assembly: RegisterMenuExtension(typeof(BlueprintAnalysisExtension))]

[assembly: RegisterAssetDefinition("LevelData", typeof(LevelDataAssetDefinition))]
[assembly: RegisterAssetDefinition("DetachedSubWorldData", typeof(DetachedSubWorldDataAssetDefinition))]
[assembly: RegisterAssetDefinition("ObjectBlueprint", typeof(ObjectBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("LogicPrefabBlueprint", typeof(LogicPrefabAssetDefinition))]
[assembly: RegisterAssetDefinition("SpatialPrefabBlueprint", typeof(SpatialPrefabAssetDefinition))]

[assembly:RegisterCustomHandler(CustomHandlerType.Res, typeof(HavokPhysicsDataActionHandler), ResType = ResourceType.HavokPhysicsData)]

// various shaders used by the level editor
[assembly: RegisterUserShader("TerrainShader", "TerrainShader")]
[assembly: RegisterUserShader("GizmoShader", "GizmoShader")]
[assembly: RegisterUserShader("SpriteShader", "SpriteShader")]
[assembly: RegisterUserShader("LevelShader", "LevelShader")]
