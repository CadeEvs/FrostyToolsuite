
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationPoseLayerData))]
	public class AnimationPoseLayer : GroupTrack, IEntityData<FrostySdk.Ebx.AnimationPoseLayerData>
	{
		public new FrostySdk.Ebx.AnimationPoseLayerData Data => data as FrostySdk.Ebx.AnimationPoseLayerData;
		public override string DisplayName => "AnimationPoseLayer";

		public AnimationPoseLayer(FrostySdk.Ebx.AnimationPoseLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

