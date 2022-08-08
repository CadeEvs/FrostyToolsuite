using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnLocationWeightGiverEntityData))]
	public class SpawnLocationWeightGiverEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.SpawnLocationWeightGiverEntityData>
	{
		public new FrostySdk.Ebx.SpawnLocationWeightGiverEntityData Data => data as FrostySdk.Ebx.SpawnLocationWeightGiverEntityData;

		public SpawnLocationWeightGiverEntity(FrostySdk.Ebx.SpawnLocationWeightGiverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

