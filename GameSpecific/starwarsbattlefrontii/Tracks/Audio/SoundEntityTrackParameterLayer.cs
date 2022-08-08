
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityTrackParameterLayerData))]
	public class SoundEntityTrackParameterLayer : SoundEntityTrackLayer, IEntityData<FrostySdk.Ebx.SoundEntityTrackParameterLayerData>
	{
		public new FrostySdk.Ebx.SoundEntityTrackParameterLayerData Data => data as FrostySdk.Ebx.SoundEntityTrackParameterLayerData;
		public override string DisplayName => "SoundEntityTrackParameterLayer";

		public SoundEntityTrackParameterLayer(FrostySdk.Ebx.SoundEntityTrackParameterLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

