using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LockingTargetSphereEntityData))]
	public class LockingTargetSphereEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LockingTargetSphereEntityData>
	{
		public new FrostySdk.Ebx.LockingTargetSphereEntityData Data => data as FrostySdk.Ebx.LockingTargetSphereEntityData;
		public override string DisplayName => "LockingTargetSphere";

		public LockingTargetSphereEntity(FrostySdk.Ebx.LockingTargetSphereEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

