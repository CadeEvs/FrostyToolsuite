
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightingTrackData))]
	public class LightingTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.LightingTrackData>
	{
		public new FrostySdk.Ebx.LightingTrackData Data => data as FrostySdk.Ebx.LightingTrackData;
		public override string DisplayName => "LightingTrack";

		public LightingTrack(FrostySdk.Ebx.LightingTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

