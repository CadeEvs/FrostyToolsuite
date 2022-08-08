using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlockVehicleDeployEntityData))]
	public class BlockVehicleDeployEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlockVehicleDeployEntityData>
	{
		public new FrostySdk.Ebx.BlockVehicleDeployEntityData Data => data as FrostySdk.Ebx.BlockVehicleDeployEntityData;
		public override string DisplayName => "BlockVehicleDeploy";

		public BlockVehicleDeployEntity(FrostySdk.Ebx.BlockVehicleDeployEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

