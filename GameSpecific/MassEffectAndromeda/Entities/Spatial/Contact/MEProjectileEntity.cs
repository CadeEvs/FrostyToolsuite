using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEProjectileEntityData))]
	public class MEProjectileEntity : BWProjectileEntity, IEntityData<FrostySdk.Ebx.MEProjectileEntityData>
	{
		public new FrostySdk.Ebx.MEProjectileEntityData Data => data as FrostySdk.Ebx.MEProjectileEntityData;

		public MEProjectileEntity(FrostySdk.Ebx.MEProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

