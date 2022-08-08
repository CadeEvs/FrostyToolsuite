
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugDashboardTrackData))]
	public class DebugDashboardTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.DebugDashboardTrackData>
	{
		public new FrostySdk.Ebx.DebugDashboardTrackData Data => data as FrostySdk.Ebx.DebugDashboardTrackData;
		public override string DisplayName => "DebugDashboardTrack";

		public DebugDashboardTrack(FrostySdk.Ebx.DebugDashboardTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

