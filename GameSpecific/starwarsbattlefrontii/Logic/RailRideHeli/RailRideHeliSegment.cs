using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RailRideHeliSegmentData))]
	public class RailRideHeliSegment : LogicEntity, IEntityData<FrostySdk.Ebx.RailRideHeliSegmentData>
	{
		public new FrostySdk.Ebx.RailRideHeliSegmentData Data => data as FrostySdk.Ebx.RailRideHeliSegmentData;
		public override string DisplayName => "RailRideHeliSegment";

		public RailRideHeliSegment(FrostySdk.Ebx.RailRideHeliSegmentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

