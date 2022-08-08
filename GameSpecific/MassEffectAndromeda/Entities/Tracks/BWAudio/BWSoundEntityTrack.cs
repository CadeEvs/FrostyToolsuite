
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWSoundEntityTrackData))]
	public class BWSoundEntityTrack : SoundEntityTrack, IEntityData<FrostySdk.Ebx.BWSoundEntityTrackData>
	{
		public new FrostySdk.Ebx.BWSoundEntityTrackData Data => data as FrostySdk.Ebx.BWSoundEntityTrackData;
		public override string DisplayName => "BWSoundEntityTrack";

		public BWSoundEntityTrack(FrostySdk.Ebx.BWSoundEntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

