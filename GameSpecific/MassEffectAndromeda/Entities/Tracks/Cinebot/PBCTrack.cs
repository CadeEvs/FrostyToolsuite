
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PBCTrackData))]
	public class PBCTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.PBCTrackData>
	{
		public new FrostySdk.Ebx.PBCTrackData Data => data as FrostySdk.Ebx.PBCTrackData;
		public override string DisplayName => "PBCTrack";

		public PBCTrack(FrostySdk.Ebx.PBCTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

