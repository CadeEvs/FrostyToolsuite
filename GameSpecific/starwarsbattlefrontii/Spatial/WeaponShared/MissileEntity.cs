using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MissileEntityData))]
	public class MissileEntity : GhostedProjectileEntity, IEntityData<FrostySdk.Ebx.MissileEntityData>
	{
		public new FrostySdk.Ebx.MissileEntityData Data => data as FrostySdk.Ebx.MissileEntityData;

		public MissileEntity(FrostySdk.Ebx.MissileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

