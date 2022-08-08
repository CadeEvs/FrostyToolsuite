
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotTransposerTrackData))]
	public class CinebotTransposerTrack : CinebotTrackableTrack, IEntityData<FrostySdk.Ebx.CinebotTransposerTrackData>
	{
		public new FrostySdk.Ebx.CinebotTransposerTrackData Data => data as FrostySdk.Ebx.CinebotTransposerTrackData;
		public override string DisplayName => "CinebotTransposerTrack";

		public CinebotTransposerTrack(FrostySdk.Ebx.CinebotTransposerTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

