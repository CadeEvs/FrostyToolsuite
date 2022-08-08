
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotTrackableTrackData))]
	public class CinebotTrackableTrack : CinebotHeirarchicalTrack, IEntityData<FrostySdk.Ebx.CinebotTrackableTrackData>
	{
		public new FrostySdk.Ebx.CinebotTrackableTrackData Data => data as FrostySdk.Ebx.CinebotTrackableTrackData;
		public override string DisplayName => "CinebotTrackableTrack";

		public CinebotTrackableTrack(FrostySdk.Ebx.CinebotTrackableTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

