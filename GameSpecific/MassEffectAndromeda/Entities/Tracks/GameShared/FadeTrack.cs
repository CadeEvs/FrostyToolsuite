
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FadeTrackData))]
	public class FadeTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.FadeTrackData>
	{
		public new FrostySdk.Ebx.FadeTrackData Data => data as FrostySdk.Ebx.FadeTrackData;
		public override string DisplayName => "Fade";
        public override string Icon => "Images/Tracks/FadeTrack.png";

        public FadeTrack(FrostySdk.Ebx.FadeTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

