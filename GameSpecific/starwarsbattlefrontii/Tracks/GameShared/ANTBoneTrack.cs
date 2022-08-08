
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTBoneTrackData))]
	public class ANTBoneTrack : LayeredTransformTrack, IEntityData<FrostySdk.Ebx.ANTBoneTrackData>
	{
		public new FrostySdk.Ebx.ANTBoneTrackData Data => data as FrostySdk.Ebx.ANTBoneTrackData;
		public override string DisplayName => "ANTBoneTrack";

		public ANTBoneTrack(FrostySdk.Ebx.ANTBoneTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

