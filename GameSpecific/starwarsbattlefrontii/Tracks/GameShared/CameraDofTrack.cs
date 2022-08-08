
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraDofTrackData))]
	public class CameraDofTrack : EntityTrackBase, IEntityData<FrostySdk.Ebx.CameraDofTrackData>
	{
		public new FrostySdk.Ebx.CameraDofTrackData Data => data as FrostySdk.Ebx.CameraDofTrackData;
		public override string DisplayName => "CameraDofTrack";

		public CameraDofTrack(FrostySdk.Ebx.CameraDofTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

