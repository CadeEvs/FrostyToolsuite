
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropTrackData))]
	public class PropTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.PropTrackData>
	{
		public new FrostySdk.Ebx.PropTrackData Data => data as FrostySdk.Ebx.PropTrackData;
		public override string DisplayName => "PropTrack";

		public PropTrack(FrostySdk.Ebx.PropTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

