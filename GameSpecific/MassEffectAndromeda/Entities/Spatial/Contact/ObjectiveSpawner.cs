using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectiveSpawnerData))]
	public class ObjectiveSpawner : SpatialEntity, IEntityData<FrostySdk.Ebx.ObjectiveSpawnerData>
	{
		public new FrostySdk.Ebx.ObjectiveSpawnerData Data => data as FrostySdk.Ebx.ObjectiveSpawnerData;

		public ObjectiveSpawner(FrostySdk.Ebx.ObjectiveSpawnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

