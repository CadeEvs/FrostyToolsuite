using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEGameplaySettingsEntityData))]
	public class MEGameplaySettingsEntity : SingletonEntity, IEntityData<FrostySdk.Ebx.MEGameplaySettingsEntityData>
	{
		public new FrostySdk.Ebx.MEGameplaySettingsEntityData Data => data as FrostySdk.Ebx.MEGameplaySettingsEntityData;
		public override string DisplayName => "MEGameplaySettings";

		public MEGameplaySettingsEntity(FrostySdk.Ebx.MEGameplaySettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

