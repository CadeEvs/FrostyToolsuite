
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2LookAtTrackData))]
	public class PA2LookAtTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.PA2LookAtTrackData>
	{
		public new FrostySdk.Ebx.PA2LookAtTrackData Data => data as FrostySdk.Ebx.PA2LookAtTrackData;
		public override string DisplayName => "PA2LookAtTrack";
        public override string Icon => "Images/Tracks/LookAtTrack.png";

        public PA2LookAtTrack(FrostySdk.Ebx.PA2LookAtTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

