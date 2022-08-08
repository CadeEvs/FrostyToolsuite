
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationPoseTrackData))]
	public class AnimationPoseTrack : LinkTrack, IEntityData<FrostySdk.Ebx.AnimationPoseTrackData>
	{
		public new FrostySdk.Ebx.AnimationPoseTrackData Data => data as FrostySdk.Ebx.AnimationPoseTrackData;
		public override string DisplayName => "AnimationPoseTrack";

		public AnimationPoseTrack(FrostySdk.Ebx.AnimationPoseTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

