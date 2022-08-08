using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleSoundEntityData))]
	public class VehicleSoundEntity : DiceSoundEntity, IEntityData<FrostySdk.Ebx.VehicleSoundEntityData>
	{
		public new FrostySdk.Ebx.VehicleSoundEntityData Data => data as FrostySdk.Ebx.VehicleSoundEntityData;
		public override string DisplayName => "VehicleSound";

		public VehicleSoundEntity(FrostySdk.Ebx.VehicleSoundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

