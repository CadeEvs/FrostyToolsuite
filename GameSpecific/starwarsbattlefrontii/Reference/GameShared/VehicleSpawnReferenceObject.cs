using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleSpawnReferenceObjectData))]
	public class VehicleSpawnReferenceObject : SpawnReferenceObject, IEntityData<FrostySdk.Ebx.VehicleSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.VehicleSpawnReferenceObjectData Data => data as FrostySdk.Ebx.VehicleSpawnReferenceObjectData;

		public VehicleSpawnReferenceObject(FrostySdk.Ebx.VehicleSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

