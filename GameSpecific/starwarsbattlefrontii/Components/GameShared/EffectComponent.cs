
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EffectComponentData))]
	public class EffectComponent : GameComponent, IEntityData<FrostySdk.Ebx.EffectComponentData>
	{
		public new FrostySdk.Ebx.EffectComponentData Data => data as FrostySdk.Ebx.EffectComponentData;
		public override string DisplayName => "EffectComponent";

		public EffectComponent(FrostySdk.Ebx.EffectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

