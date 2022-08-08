using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoftLockEntityData))]
	public class SoftLockEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoftLockEntityData>
	{
		public new FrostySdk.Ebx.SoftLockEntityData Data => data as FrostySdk.Ebx.SoftLockEntityData;
		public override string DisplayName => "SoftLock";

		public SoftLockEntity(FrostySdk.Ebx.SoftLockEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

