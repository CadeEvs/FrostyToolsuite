using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnPointGroupSelectorEntityData))]
	public class SpawnPointGroupSelectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnPointGroupSelectorEntityData>
	{
		public new FrostySdk.Ebx.SpawnPointGroupSelectorEntityData Data => data as FrostySdk.Ebx.SpawnPointGroupSelectorEntityData;
		public override string DisplayName => "SpawnPointGroupSelector";

		public SpawnPointGroupSelectorEntity(FrostySdk.Ebx.SpawnPointGroupSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

