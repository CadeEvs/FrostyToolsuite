using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugPlayerSpawnerData))]
	public class DebugPlayerSpawner : LogicEntity, IEntityData<FrostySdk.Ebx.DebugPlayerSpawnerData>
	{
		public new FrostySdk.Ebx.DebugPlayerSpawnerData Data => data as FrostySdk.Ebx.DebugPlayerSpawnerData;
		public override string DisplayName => "DebugPlayerSpawner";

		public DebugPlayerSpawner(FrostySdk.Ebx.DebugPlayerSpawnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

