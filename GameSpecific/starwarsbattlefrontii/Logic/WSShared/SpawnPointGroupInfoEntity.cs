using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnPointGroupInfoEntityData))]
	public class SpawnPointGroupInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnPointGroupInfoEntityData>
	{
		public new FrostySdk.Ebx.SpawnPointGroupInfoEntityData Data => data as FrostySdk.Ebx.SpawnPointGroupInfoEntityData;
		public override string DisplayName => "SpawnPointGroupInfo";

		public SpawnPointGroupInfoEntity(FrostySdk.Ebx.SpawnPointGroupInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

