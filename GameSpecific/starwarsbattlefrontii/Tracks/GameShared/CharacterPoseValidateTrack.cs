
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterPoseValidateTrackData))]
	public class CharacterPoseValidateTrack : PoseValidateTrack, IEntityData<FrostySdk.Ebx.CharacterPoseValidateTrackData>
	{
		public new FrostySdk.Ebx.CharacterPoseValidateTrackData Data => data as FrostySdk.Ebx.CharacterPoseValidateTrackData;
		public override string DisplayName => "CharacterPoseValidateTrack";

		public CharacterPoseValidateTrack(FrostySdk.Ebx.CharacterPoseValidateTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

