
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightTrackData))]
	public class LightTrack : EntityTrackBase, IEntityData<FrostySdk.Ebx.LightTrackData>
	{
		public new FrostySdk.Ebx.LightTrackData Data => data as FrostySdk.Ebx.LightTrackData;
		public override string DisplayName => "LightTrack";

		public LightTrack(FrostySdk.Ebx.LightTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

