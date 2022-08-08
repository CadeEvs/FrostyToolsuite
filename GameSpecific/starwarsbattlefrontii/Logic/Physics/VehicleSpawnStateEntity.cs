using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleSpawnStateEntityData))]
	public class VehicleSpawnStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VehicleSpawnStateEntityData>
	{
		public new FrostySdk.Ebx.VehicleSpawnStateEntityData Data => data as FrostySdk.Ebx.VehicleSpawnStateEntityData;
		public override string DisplayName => "VehicleSpawnState";

		public VehicleSpawnStateEntity(FrostySdk.Ebx.VehicleSpawnStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

