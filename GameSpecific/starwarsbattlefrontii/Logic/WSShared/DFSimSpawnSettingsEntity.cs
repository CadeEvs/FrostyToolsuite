using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DFSimSpawnSettingsEntityData))]
	public class DFSimSpawnSettingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DFSimSpawnSettingsEntityData>
	{
		public new FrostySdk.Ebx.DFSimSpawnSettingsEntityData Data => data as FrostySdk.Ebx.DFSimSpawnSettingsEntityData;
		public override string DisplayName => "DFSimSpawnSettings";

		public DFSimSpawnSettingsEntity(FrostySdk.Ebx.DFSimSpawnSettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

