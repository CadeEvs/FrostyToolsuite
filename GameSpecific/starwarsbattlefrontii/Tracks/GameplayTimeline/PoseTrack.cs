
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PoseTrackData))]
	public class PoseTrack : ANTLayer, IEntityData<FrostySdk.Ebx.PoseTrackData>
	{
		public new FrostySdk.Ebx.PoseTrackData Data => data as FrostySdk.Ebx.PoseTrackData;
		public override string DisplayName => "PoseTrack";

		public PoseTrack(FrostySdk.Ebx.PoseTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

