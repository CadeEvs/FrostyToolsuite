
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityTrackAssetLayerData))]
	public class SoundEntityTrackAssetLayer : SoundEntityTrackLayer, IEntityData<FrostySdk.Ebx.SoundEntityTrackAssetLayerData>
	{
		public new FrostySdk.Ebx.SoundEntityTrackAssetLayerData Data => data as FrostySdk.Ebx.SoundEntityTrackAssetLayerData;
		public override string DisplayName => "SoundEntityTrackAssetLayer";

		public SoundEntityTrackAssetLayer(FrostySdk.Ebx.SoundEntityTrackAssetLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

