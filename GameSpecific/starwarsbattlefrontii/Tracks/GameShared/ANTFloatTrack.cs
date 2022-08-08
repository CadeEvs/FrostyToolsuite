
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTFloatTrackData))]
	public class ANTFloatTrack : FloatTrack, IEntityData<FrostySdk.Ebx.ANTFloatTrackData>
	{
		public new FrostySdk.Ebx.ANTFloatTrackData Data => data as FrostySdk.Ebx.ANTFloatTrackData;
		public override string DisplayName => "ANTFloatTrack";

		public ANTFloatTrack(FrostySdk.Ebx.ANTFloatTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

