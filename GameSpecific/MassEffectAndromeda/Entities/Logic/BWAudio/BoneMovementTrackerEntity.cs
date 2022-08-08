using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoneMovementTrackerEntityData))]
	public class BoneMovementTrackerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BoneMovementTrackerEntityData>
	{
		public new FrostySdk.Ebx.BoneMovementTrackerEntityData Data => data as FrostySdk.Ebx.BoneMovementTrackerEntityData;
		public override string DisplayName => "BoneMovementTracker";

		public BoneMovementTrackerEntity(FrostySdk.Ebx.BoneMovementTrackerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

