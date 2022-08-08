using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSMissileEntityData))]
	public class WSMissileEntity : MissileEntity, IEntityData<FrostySdk.Ebx.WSMissileEntityData>
	{
		public new FrostySdk.Ebx.WSMissileEntityData Data => data as FrostySdk.Ebx.WSMissileEntityData;

		public WSMissileEntity(FrostySdk.Ebx.WSMissileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

