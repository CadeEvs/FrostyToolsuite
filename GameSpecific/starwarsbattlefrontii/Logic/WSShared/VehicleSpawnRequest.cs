using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleSpawnRequestData))]
	public class VehicleSpawnRequest : LogicEntity, IEntityData<FrostySdk.Ebx.VehicleSpawnRequestData>
	{
		public new FrostySdk.Ebx.VehicleSpawnRequestData Data => data as FrostySdk.Ebx.VehicleSpawnRequestData;
		public override string DisplayName => "VehicleSpawnRequest";

		public VehicleSpawnRequest(FrostySdk.Ebx.VehicleSpawnRequestData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

