using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubObjectiveSpawnerData))]
	public class SubObjectiveSpawner : LogicEntity, IEntityData<FrostySdk.Ebx.SubObjectiveSpawnerData>
	{
		public new FrostySdk.Ebx.SubObjectiveSpawnerData Data => data as FrostySdk.Ebx.SubObjectiveSpawnerData;
		public override string DisplayName => "SubObjectiveSpawner";

		public SubObjectiveSpawner(FrostySdk.Ebx.SubObjectiveSpawnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

