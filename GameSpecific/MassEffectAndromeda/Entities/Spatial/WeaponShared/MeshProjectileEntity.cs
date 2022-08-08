using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshProjectileEntityData))]
	public class MeshProjectileEntity : ProjectileEntity, IEntityData<FrostySdk.Ebx.MeshProjectileEntityData>
	{
		public new FrostySdk.Ebx.MeshProjectileEntityData Data => data as FrostySdk.Ebx.MeshProjectileEntityData;

		public MeshProjectileEntity(FrostySdk.Ebx.MeshProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

