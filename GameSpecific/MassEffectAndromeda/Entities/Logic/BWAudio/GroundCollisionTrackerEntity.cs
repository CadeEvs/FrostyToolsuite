using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroundCollisionTrackerEntityData))]
	public class GroundCollisionTrackerEntity : BoneMovementTrackerEntity, IEntityData<FrostySdk.Ebx.GroundCollisionTrackerEntityData>
	{
		public new FrostySdk.Ebx.GroundCollisionTrackerEntityData Data => data as FrostySdk.Ebx.GroundCollisionTrackerEntityData;
		public override string DisplayName => "GroundCollisionTracker";

		public GroundCollisionTrackerEntity(FrostySdk.Ebx.GroundCollisionTrackerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

