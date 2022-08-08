
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RgbColorTrackData))]
	public class RgbColorTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.RgbColorTrackData>
	{
		public new FrostySdk.Ebx.RgbColorTrackData Data => data as FrostySdk.Ebx.RgbColorTrackData;
		public override string DisplayName => "RgbColorTrack";

		public RgbColorTrack(FrostySdk.Ebx.RgbColorTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

