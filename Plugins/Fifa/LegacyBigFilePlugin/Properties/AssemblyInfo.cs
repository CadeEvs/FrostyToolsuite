﻿using Frosty.Core.Attributes;
using FrostySdk;
using LegacyBigFilePlugin;
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
[assembly: Guid("3ebc36b1-024a-44eb-b0db-607dd7fcee6c")]

[assembly: PluginDisplayName("Legacy BIG Editor")]
[assembly: PluginAuthor("GalaxyMan2015")]
[assembly: PluginVersion("1.0.0.0")]

[assembly: PluginValidForProfile((int)ProfileVersion.Fifa17)]
[assembly: PluginValidForProfile((int)ProfileVersion.Fifa18)]
[assembly: PluginValidForProfile((int)ProfileVersion.Fifa19)]
[assembly: PluginValidForProfile((int)ProfileVersion.Fifa20)]
[assembly: PluginValidForProfile((int)ProfileVersion.Madden20)]
[assembly: PluginValidForProfile((int)ProfileVersion.Fifa21)]
[assembly: PluginValidForProfile((int)ProfileVersion.Madden22)]
[assembly: PluginValidForProfile((int)ProfileVersion.Fifa22)]
[assembly: PluginValidForProfile((int)ProfileVersion.Madden23)]
[assembly: PluginValidForProfile((int)ProfileVersion.PGA)]

[assembly: RegisterAssetDefinition("BIG", typeof(BigFileAssetDefinition))]
[assembly: RegisterAssetDefinition("AST", typeof(BigFileAssetDefinition))]