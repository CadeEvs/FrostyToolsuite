using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetLockingFilterEntityData))]
	public class TargetLockingFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetLockingFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetLockingFilterEntityData Data => data as FrostySdk.Ebx.TargetLockingFilterEntityData;
		public override string DisplayName => "TargetLockingFilter";

		public TargetLockingFilterEntity(FrostySdk.Ebx.TargetLockingFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

