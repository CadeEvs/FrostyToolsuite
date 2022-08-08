using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HealthTransitionSpawnReferenceObjectData))]
	public class HealthTransitionSpawnReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.HealthTransitionSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.HealthTransitionSpawnReferenceObjectData Data => data as FrostySdk.Ebx.HealthTransitionSpawnReferenceObjectData;

		public HealthTransitionSpawnReferenceObject(FrostySdk.Ebx.HealthTransitionSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

