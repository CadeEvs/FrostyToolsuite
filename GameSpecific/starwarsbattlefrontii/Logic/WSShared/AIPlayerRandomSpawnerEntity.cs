using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPlayerRandomSpawnerEntityData))]
	public class AIPlayerRandomSpawnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIPlayerRandomSpawnerEntityData>
	{
		public new FrostySdk.Ebx.AIPlayerRandomSpawnerEntityData Data => data as FrostySdk.Ebx.AIPlayerRandomSpawnerEntityData;
		public override string DisplayName => "AIPlayerRandomSpawner";

		public AIPlayerRandomSpawnerEntity(FrostySdk.Ebx.AIPlayerRandomSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

