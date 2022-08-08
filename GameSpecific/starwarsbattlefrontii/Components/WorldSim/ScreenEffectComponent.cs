
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenEffectComponentData))]
	public class ScreenEffectComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ScreenEffectComponentData>
	{
		public new FrostySdk.Ebx.ScreenEffectComponentData Data => data as FrostySdk.Ebx.ScreenEffectComponentData;
		public override string DisplayName => "ScreenEffectComponent";

		public ScreenEffectComponent(FrostySdk.Ebx.ScreenEffectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

