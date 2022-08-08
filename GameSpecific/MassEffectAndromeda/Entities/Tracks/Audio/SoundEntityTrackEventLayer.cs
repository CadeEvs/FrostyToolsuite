
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityTrackEventLayerData))]
	public class SoundEntityTrackEventLayer : SoundEntityTrackLayer, IEntityData<FrostySdk.Ebx.SoundEntityTrackEventLayerData>
	{
		public new FrostySdk.Ebx.SoundEntityTrackEventLayerData Data => data as FrostySdk.Ebx.SoundEntityTrackEventLayerData;
		public override string DisplayName => "SoundEntityTrackEventLayer";

		public SoundEntityTrackEventLayer(FrostySdk.Ebx.SoundEntityTrackEventLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

