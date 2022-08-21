using Frosty.Core.Attributes;
using SoundEditorPlugin;
using SoundEditorPlugin.Resources;
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
[assembly: Guid("5fdd1243-084e-42dc-99bb-394a169d988f")]

[assembly: PluginDisplayName("Sound Editor")]
[assembly: PluginAuthor("GalaxyMan2015 & wannkunstbeikor")]
[assembly: PluginVersion("1.0.0.1")]

[assembly: RegisterOptionsExtension(typeof(SoundOptions))]

[assembly: RegisterTypeOverride("LocalizedWaveAsset", typeof(LocalizedWaveAssetOverride))]
[assembly: RegisterTypeOverride("NewWaveAsset", typeof(NewWaveAssetOverride))]

[assembly: RegisterAssetDefinition("SoundWaveAsset", typeof(SoundWaveAssetDefinition))]
[assembly: RegisterAssetDefinition("NewWaveAsset", typeof(NewWaveAssetDefinition))]
[assembly: RegisterAssetDefinition("HarmonySampleBankAsset", typeof(HarmonySampleBankAssetDefinition))]
[assembly: RegisterAssetDefinition("OctaneAsset", typeof(OctaneAssetDefinition))]
[assembly: RegisterAssetDefinition("ImpulseResponseAsset", typeof(ImpulseResponseAssetDefinition))]

[assembly: RegisterThirdPartyDll("NAudio")]
[assembly: RegisterThirdPartyDll("NAudio.WaveFormRenderer")]
