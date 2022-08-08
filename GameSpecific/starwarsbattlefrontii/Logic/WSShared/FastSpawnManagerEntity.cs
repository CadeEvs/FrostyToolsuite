using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FastSpawnManagerEntityData))]
	public class FastSpawnManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FastSpawnManagerEntityData>
	{
		public new FrostySdk.Ebx.FastSpawnManagerEntityData Data => data as FrostySdk.Ebx.FastSpawnManagerEntityData;
		public override string DisplayName => "FastSpawnManager";

		public FastSpawnManagerEntity(FrostySdk.Ebx.FastSpawnManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

