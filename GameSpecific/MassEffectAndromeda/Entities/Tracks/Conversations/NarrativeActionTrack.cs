
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NarrativeActionTrackData))]
	public class NarrativeActionTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.NarrativeActionTrackData>
	{
		public new FrostySdk.Ebx.NarrativeActionTrackData Data => data as FrostySdk.Ebx.NarrativeActionTrackData;
		public override string DisplayName => "NarrativeActionTrack";

		public NarrativeActionTrack(FrostySdk.Ebx.NarrativeActionTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

