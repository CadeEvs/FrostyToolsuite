
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTIntTrackData))]
	public class ANTIntTrack : IntTrack, IEntityData<FrostySdk.Ebx.ANTIntTrackData>
	{
		public new FrostySdk.Ebx.ANTIntTrackData Data => data as FrostySdk.Ebx.ANTIntTrackData;
		public override string DisplayName => "ANTIntTrack";

		public ANTIntTrack(FrostySdk.Ebx.ANTIntTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

