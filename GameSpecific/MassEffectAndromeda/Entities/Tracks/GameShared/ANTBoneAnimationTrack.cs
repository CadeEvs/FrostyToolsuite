
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTBoneAnimationTrackData))]
	public class ANTBoneAnimationTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.ANTBoneAnimationTrackData>
	{
		public new FrostySdk.Ebx.ANTBoneAnimationTrackData Data => data as FrostySdk.Ebx.ANTBoneAnimationTrackData;
		public override string DisplayName => "ANTBoneAnimationTrack";

		public ANTBoneAnimationTrack(FrostySdk.Ebx.ANTBoneAnimationTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

