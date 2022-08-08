using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HDRBrightnessSettingsEntityData))]
	public class HDRBrightnessSettingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HDRBrightnessSettingsEntityData>
	{
		public new FrostySdk.Ebx.HDRBrightnessSettingsEntityData Data => data as FrostySdk.Ebx.HDRBrightnessSettingsEntityData;
		public override string DisplayName => "HDRBrightnessSettings";

		public HDRBrightnessSettingsEntity(FrostySdk.Ebx.HDRBrightnessSettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

