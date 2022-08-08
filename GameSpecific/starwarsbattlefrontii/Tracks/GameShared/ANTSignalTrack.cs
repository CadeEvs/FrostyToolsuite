
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTSignalTrackData))]
	public class ANTSignalTrack : LinkTrack, IEntityData<FrostySdk.Ebx.ANTSignalTrackData>
	{
		public new FrostySdk.Ebx.ANTSignalTrackData Data => data as FrostySdk.Ebx.ANTSignalTrackData;
		public override string DisplayName => "ANTSignalTrack";

		public ANTSignalTrack(FrostySdk.Ebx.ANTSignalTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

