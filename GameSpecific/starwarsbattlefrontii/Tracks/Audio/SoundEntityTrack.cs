
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityTrackData))]
	public class SoundEntityTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.SoundEntityTrackData>
	{
		public new FrostySdk.Ebx.SoundEntityTrackData Data => data as FrostySdk.Ebx.SoundEntityTrackData;
		public override string DisplayName => "SoundEntityTrack";

		public SoundEntityTrack(FrostySdk.Ebx.SoundEntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

