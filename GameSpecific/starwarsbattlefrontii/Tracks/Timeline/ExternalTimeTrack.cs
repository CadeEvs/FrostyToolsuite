
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExternalTimeTrackData))]
	public class ExternalTimeTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.ExternalTimeTrackData>
	{
		public new FrostySdk.Ebx.ExternalTimeTrackData Data => data as FrostySdk.Ebx.ExternalTimeTrackData;
		public override string DisplayName => "ExternalTimeTrack";

		public ExternalTimeTrack(FrostySdk.Ebx.ExternalTimeTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

