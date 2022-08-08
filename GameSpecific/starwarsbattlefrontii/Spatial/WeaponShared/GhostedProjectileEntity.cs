using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GhostedProjectileEntityData))]
	public class GhostedProjectileEntity : MeshProjectileEntity, IEntityData<FrostySdk.Ebx.GhostedProjectileEntityData>
	{
		public new FrostySdk.Ebx.GhostedProjectileEntityData Data => data as FrostySdk.Ebx.GhostedProjectileEntityData;

		public GhostedProjectileEntity(FrostySdk.Ebx.GhostedProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

