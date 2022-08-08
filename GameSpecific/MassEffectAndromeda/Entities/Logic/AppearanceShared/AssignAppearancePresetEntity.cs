using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AssignAppearancePresetEntityData))]
	public class AssignAppearancePresetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AssignAppearancePresetEntityData>
	{
		public new FrostySdk.Ebx.AssignAppearancePresetEntityData Data => data as FrostySdk.Ebx.AssignAppearancePresetEntityData;
		public override string DisplayName => "AssignAppearancePreset";

		public AssignAppearancePresetEntity(FrostySdk.Ebx.AssignAppearancePresetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

