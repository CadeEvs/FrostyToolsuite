
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverAnimationComponentData))]
	public class VoiceOverAnimationComponent : GameComponent, IEntityData<FrostySdk.Ebx.VoiceOverAnimationComponentData>
	{
		public new FrostySdk.Ebx.VoiceOverAnimationComponentData Data => data as FrostySdk.Ebx.VoiceOverAnimationComponentData;
		public override string DisplayName => "VoiceOverAnimationComponent";

		public VoiceOverAnimationComponent(FrostySdk.Ebx.VoiceOverAnimationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

