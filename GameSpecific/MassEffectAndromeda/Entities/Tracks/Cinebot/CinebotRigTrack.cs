
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotRigTrackData))]
	public class CinebotRigTrack : CinebotHeirarchicalTrack, IEntityData<FrostySdk.Ebx.CinebotRigTrackData>
	{
		public new FrostySdk.Ebx.CinebotRigTrackData Data => data as FrostySdk.Ebx.CinebotRigTrackData;
		public override string DisplayName => "CinebotRigTrack";

		public CinebotRigTrack(FrostySdk.Ebx.CinebotRigTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

