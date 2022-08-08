using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProjectileEntityData))]
	public class ProjectileEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.ProjectileEntityData>
	{
		public new FrostySdk.Ebx.ProjectileEntityData Data => data as FrostySdk.Ebx.ProjectileEntityData;

		public ProjectileEntity(FrostySdk.Ebx.ProjectileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

