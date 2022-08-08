using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleProjectileEntityData))]
	public class VehicleProjectileEntity : ProjectileEntity, IEntityData<FrostySdk.Ebx.VehicleProjectileEntityData>
	{
		public new FrostySdk.Ebx.VehicleProjectileEntityData Data => data as FrostySdk.Ebx.VehicleProjectileEntityData;

		public VehicleProjectileEntity(FrostySdk.Ebx.VehicleProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

