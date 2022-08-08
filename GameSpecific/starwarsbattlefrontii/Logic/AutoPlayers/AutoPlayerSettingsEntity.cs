using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerSettingsEntityData))]
	public class AutoPlayerSettingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutoPlayerSettingsEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerSettingsEntityData Data => data as FrostySdk.Ebx.AutoPlayerSettingsEntityData;
		public override string DisplayName => "AutoPlayerSettings";

		public AutoPlayerSettingsEntity(FrostySdk.Ebx.AutoPlayerSettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

