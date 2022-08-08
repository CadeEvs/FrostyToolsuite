using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LockingTargetEntityData))]
	public class LockingTargetEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LockingTargetEntityData>
	{
		public new FrostySdk.Ebx.LockingTargetEntityData Data => data as FrostySdk.Ebx.LockingTargetEntityData;

		public LockingTargetEntity(FrostySdk.Ebx.LockingTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

