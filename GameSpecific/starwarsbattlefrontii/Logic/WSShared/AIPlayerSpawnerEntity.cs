using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPlayerSpawnerEntityData))]
	public class AIPlayerSpawnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIPlayerSpawnerEntityData>
	{
		public new FrostySdk.Ebx.AIPlayerSpawnerEntityData Data => data as FrostySdk.Ebx.AIPlayerSpawnerEntityData;
		public override string DisplayName => "AIPlayerSpawner";

		public AIPlayerSpawnerEntity(FrostySdk.Ebx.AIPlayerSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

