
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FacialEmotionTrackData))]
	public class FacialEmotionTrack : LinkTrack, IEntityData<FrostySdk.Ebx.FacialEmotionTrackData>
	{
		public new FrostySdk.Ebx.FacialEmotionTrackData Data => data as FrostySdk.Ebx.FacialEmotionTrackData;
		public override string DisplayName => "FacialEmotionTrack";
		public override string Icon => "Images/Tracks/FaceEmotionTrack.png";

        public FacialEmotionTrack(FrostySdk.Ebx.FacialEmotionTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

