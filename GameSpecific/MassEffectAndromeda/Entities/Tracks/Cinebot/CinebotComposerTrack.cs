
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotComposerTrackData))]
	public class CinebotComposerTrack : CinebotTrackableTrack, IEntityData<FrostySdk.Ebx.CinebotComposerTrackData>
	{
		public new FrostySdk.Ebx.CinebotComposerTrackData Data => data as FrostySdk.Ebx.CinebotComposerTrackData;
		public override string DisplayName => "CinebotComposerTrack";

		public CinebotComposerTrack(FrostySdk.Ebx.CinebotComposerTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

