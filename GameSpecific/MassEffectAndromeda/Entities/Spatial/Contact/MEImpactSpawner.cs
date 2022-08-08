using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEImpactSpawnerData))]
	public class MEImpactSpawner : SpatialEntity, IEntityData<FrostySdk.Ebx.MEImpactSpawnerData>
	{
		public new FrostySdk.Ebx.MEImpactSpawnerData Data => data as FrostySdk.Ebx.MEImpactSpawnerData;

		public MEImpactSpawner(FrostySdk.Ebx.MEImpactSpawnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

