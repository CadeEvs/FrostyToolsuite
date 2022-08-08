using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleEntityData))]
	public class VehicleEntity : ControllableEntity, IEntityData<FrostySdk.Ebx.VehicleEntityData>
	{
		public new FrostySdk.Ebx.VehicleEntityData Data => data as FrostySdk.Ebx.VehicleEntityData;

		public VehicleEntity(FrostySdk.Ebx.VehicleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

