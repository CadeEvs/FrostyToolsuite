
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinkTrackData))]
	public class LinkTrack : SchematicPinTrack, IEntityData<FrostySdk.Ebx.LinkTrackData>
	{
		public new FrostySdk.Ebx.LinkTrackData Data => data as FrostySdk.Ebx.LinkTrackData;
		public override string DisplayName => "LinkTrack";

		public LinkTrack(FrostySdk.Ebx.LinkTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

