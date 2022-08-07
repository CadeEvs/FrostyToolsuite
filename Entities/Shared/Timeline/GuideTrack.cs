
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GuideTrackData))]
	public class GuideTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.GuideTrackData>
	{
		public new FrostySdk.Ebx.GuideTrackData Data => data as FrostySdk.Ebx.GuideTrackData;
		public override string DisplayName => "GuideTrack";

		public GuideTrack(FrostySdk.Ebx.GuideTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

