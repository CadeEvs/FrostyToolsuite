
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotFloatTrackData))]
	public class CinebotFloatTrack : FloatTrack, IEntityData<FrostySdk.Ebx.CinebotFloatTrackData>
	{
		public new FrostySdk.Ebx.CinebotFloatTrackData Data => data as FrostySdk.Ebx.CinebotFloatTrackData;
		public override string DisplayName => "CinebotFloatTrack";

		public CinebotFloatTrack(FrostySdk.Ebx.CinebotFloatTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

