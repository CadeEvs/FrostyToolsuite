using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleSpawnReferenceObjectData))]
	public class VehicleSpawnReferenceObject : SpawnReferenceObject, IEntityData<FrostySdk.Ebx.VehicleSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.VehicleSpawnReferenceObjectData Data => data as FrostySdk.Ebx.VehicleSpawnReferenceObjectData;
		public new Assets.VehicleBlueprint Blueprint => blueprint as Assets.VehicleBlueprint;
		public new VehicleEntity RootEntity => entities[0] as VehicleEntity;

		public VehicleSpawnReferenceObject(FrostySdk.Ebx.VehicleSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

