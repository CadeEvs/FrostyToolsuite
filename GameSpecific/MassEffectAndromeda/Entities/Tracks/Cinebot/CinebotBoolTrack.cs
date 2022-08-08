
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotBoolTrackData))]
	public class CinebotBoolTrack : BoolTrack, IEntityData<FrostySdk.Ebx.CinebotBoolTrackData>
	{
		public new FrostySdk.Ebx.CinebotBoolTrackData Data => data as FrostySdk.Ebx.CinebotBoolTrackData;
		public override string DisplayName => "CinebotBoolTrack";

		public CinebotBoolTrack(FrostySdk.Ebx.CinebotBoolTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

