using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWProjectileEntityData))]
	public class BWProjectileEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.BWProjectileEntityData>
	{
		public new FrostySdk.Ebx.BWProjectileEntityData Data => data as FrostySdk.Ebx.BWProjectileEntityData;

		public BWProjectileEntity(FrostySdk.Ebx.BWProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

