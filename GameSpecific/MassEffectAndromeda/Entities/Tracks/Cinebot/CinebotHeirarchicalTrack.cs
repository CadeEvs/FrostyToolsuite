
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotHeirarchicalTrackData))]
	public class CinebotHeirarchicalTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.CinebotHeirarchicalTrackData>
	{
		public new FrostySdk.Ebx.CinebotHeirarchicalTrackData Data => data as FrostySdk.Ebx.CinebotHeirarchicalTrackData;
		public override string DisplayName => "CinebotHeirarchicalTrack";

		public CinebotHeirarchicalTrack(FrostySdk.Ebx.CinebotHeirarchicalTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

