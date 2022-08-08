
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterPoseRecordTrackData))]
	public class CharacterPoseRecordTrack : PoseRecordTrack, IEntityData<FrostySdk.Ebx.CharacterPoseRecordTrackData>
	{
		public new FrostySdk.Ebx.CharacterPoseRecordTrackData Data => data as FrostySdk.Ebx.CharacterPoseRecordTrackData;
		public override string DisplayName => "CharacterPoseRecordTrack";

		public CharacterPoseRecordTrack(FrostySdk.Ebx.CharacterPoseRecordTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

