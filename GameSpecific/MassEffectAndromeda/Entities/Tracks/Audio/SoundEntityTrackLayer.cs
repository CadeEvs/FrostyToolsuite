
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityTrackLayerData))]
	public class SoundEntityTrackLayer : TimelineTrack, IEntityData<FrostySdk.Ebx.SoundEntityTrackLayerData>
	{
		public new FrostySdk.Ebx.SoundEntityTrackLayerData Data => data as FrostySdk.Ebx.SoundEntityTrackLayerData;
		public override string DisplayName => "SoundEntityTrackLayer";

		public SoundEntityTrackLayer(FrostySdk.Ebx.SoundEntityTrackLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

