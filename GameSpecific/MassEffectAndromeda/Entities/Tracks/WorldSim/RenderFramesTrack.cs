
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RenderFramesTrackData))]
	public class RenderFramesTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.RenderFramesTrackData>
	{
		public new FrostySdk.Ebx.RenderFramesTrackData Data => data as FrostySdk.Ebx.RenderFramesTrackData;
		public override string DisplayName => "RenderFramesTrack";

		public RenderFramesTrack(FrostySdk.Ebx.RenderFramesTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

