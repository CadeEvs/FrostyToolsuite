
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityTrackTransformLayerData))]
	public class SoundEntityTrackTransformLayer : LayeredTransformTrack, IEntityData<FrostySdk.Ebx.SoundEntityTrackTransformLayerData>
	{
		public new FrostySdk.Ebx.SoundEntityTrackTransformLayerData Data => data as FrostySdk.Ebx.SoundEntityTrackTransformLayerData;
		public override string DisplayName => "SoundEntityTrackTransformLayer";

		public SoundEntityTrackTransformLayer(FrostySdk.Ebx.SoundEntityTrackTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

