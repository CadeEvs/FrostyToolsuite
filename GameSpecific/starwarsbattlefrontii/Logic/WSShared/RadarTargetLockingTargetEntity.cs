using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarTargetLockingTargetEntityData))]
	public class RadarTargetLockingTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadarTargetLockingTargetEntityData>
	{
		public new FrostySdk.Ebx.RadarTargetLockingTargetEntityData Data => data as FrostySdk.Ebx.RadarTargetLockingTargetEntityData;
		public override string DisplayName => "RadarTargetLockingTarget";

		public RadarTargetLockingTargetEntity(FrostySdk.Ebx.RadarTargetLockingTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

