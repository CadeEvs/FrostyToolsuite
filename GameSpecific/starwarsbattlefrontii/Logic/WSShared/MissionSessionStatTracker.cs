using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MissionSessionStatTrackerData))]
	public class MissionSessionStatTracker : LogicEntity, IEntityData<FrostySdk.Ebx.MissionSessionStatTrackerData>
	{
		public new FrostySdk.Ebx.MissionSessionStatTrackerData Data => data as FrostySdk.Ebx.MissionSessionStatTrackerData;
		public override string DisplayName => "MissionSessionStatTracker";

		public MissionSessionStatTracker(FrostySdk.Ebx.MissionSessionStatTrackerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

