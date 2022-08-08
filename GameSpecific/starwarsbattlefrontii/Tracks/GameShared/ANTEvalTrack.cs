
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTEvalTrackData))]
	public class ANTEvalTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.ANTEvalTrackData>
	{
		public new FrostySdk.Ebx.ANTEvalTrackData Data => data as FrostySdk.Ebx.ANTEvalTrackData;
		public override string DisplayName => "ANTEvalTrack";

		public ANTEvalTrack(FrostySdk.Ebx.ANTEvalTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

