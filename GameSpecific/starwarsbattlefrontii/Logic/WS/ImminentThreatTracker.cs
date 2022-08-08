using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ImminentThreatTrackerData))]
	public class ImminentThreatTracker : LogicEntity, IEntityData<FrostySdk.Ebx.ImminentThreatTrackerData>
	{
		public new FrostySdk.Ebx.ImminentThreatTrackerData Data => data as FrostySdk.Ebx.ImminentThreatTrackerData;
		public override string DisplayName => "ImminentThreatTracker";

		public ImminentThreatTracker(FrostySdk.Ebx.ImminentThreatTrackerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

