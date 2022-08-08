using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerVehicleProximityEntityData))]
	public class PlayerVehicleProximityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerVehicleProximityEntityData>
	{
		public new FrostySdk.Ebx.PlayerVehicleProximityEntityData Data => data as FrostySdk.Ebx.PlayerVehicleProximityEntityData;
		public override string DisplayName => "PlayerVehicleProximity";

		public PlayerVehicleProximityEntity(FrostySdk.Ebx.PlayerVehicleProximityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

