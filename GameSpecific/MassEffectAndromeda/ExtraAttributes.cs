using LevelEditorPlugin.Definitions;
using LevelEditorPlugin.Extensions;
using Frosty.Core.Attributes;
using FrostySdk;

// Extra attributes are placed here that are specific to this game

[assembly: RegisterStartupAction(typeof(LoadItemsStartupAction))]
[assembly: PluginValidForProfile((int)ProfileVersion.MassEffectAndromeda)]

// logic blueprints
[assembly: RegisterAssetDefinition("ActionLogicPrefabBlueprint", typeof(ActionLogicPrefabAssetDefinition))]
[assembly: RegisterAssetDefinition("ConditionLogicPrefabBlueprint", typeof(ConditionLogicPrefabAssetDefinition))]

// object blueprints
[assembly: RegisterAssetDefinition("AppearanceEntityBlueprint", typeof(AppearanceEntityBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("BWBeamBlueprint", typeof(BWBeamBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("MEProjectileBlueprint", typeof(MEProjectileBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("GameObjectBlueprint", typeof(GameObjectBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("UIWidgetBlueprint", typeof(UIWidgetBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("MESoldierBlueprint", typeof(MESoldierBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("EffectBlueprint", typeof(EffectBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("VehicleBlueprint", typeof(VehicleBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("VisualEnvironmentBlueprint", typeof(VisualEnvironmentBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("LensFlareBlueprint", typeof(LensFlareBlueprintAssetDefinition))]

// other
[assembly: RegisterAssetDefinition("MEPowerBlueprint", typeof(MEPowerBlueprintAssetDefinition))]
[assembly: RegisterAssetDefinition("LayerData", typeof(LayerDataAssetDefinition))]
[assembly: RegisterAssetDefinition("SubWorldData", typeof(SubWorldDataAssetDefinition))]