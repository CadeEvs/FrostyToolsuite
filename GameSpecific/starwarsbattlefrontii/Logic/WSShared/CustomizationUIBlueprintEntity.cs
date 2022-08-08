using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomizationUIBlueprintEntityData))]
	public class CustomizationUIBlueprintEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CustomizationUIBlueprintEntityData>
	{
		public new FrostySdk.Ebx.CustomizationUIBlueprintEntityData Data => data as FrostySdk.Ebx.CustomizationUIBlueprintEntityData;
		public override string DisplayName => "CustomizationUIBlueprint";

		public CustomizationUIBlueprintEntity(FrostySdk.Ebx.CustomizationUIBlueprintEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

