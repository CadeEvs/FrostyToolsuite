using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnPointGroupEntityData))]
	public class SpawnPointGroupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnPointGroupEntityData>
	{
		public new FrostySdk.Ebx.SpawnPointGroupEntityData Data => data as FrostySdk.Ebx.SpawnPointGroupEntityData;
		public override string DisplayName => "SpawnPointGroup";

		public SpawnPointGroupEntity(FrostySdk.Ebx.SpawnPointGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

