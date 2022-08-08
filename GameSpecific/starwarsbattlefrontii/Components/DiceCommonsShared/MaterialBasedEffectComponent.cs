
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MaterialBasedEffectComponentData))]
	public class MaterialBasedEffectComponent : GameComponent, IEntityData<FrostySdk.Ebx.MaterialBasedEffectComponentData>
	{
		public new FrostySdk.Ebx.MaterialBasedEffectComponentData Data => data as FrostySdk.Ebx.MaterialBasedEffectComponentData;
		public override string DisplayName => "MaterialBasedEffectComponent";

		public MaterialBasedEffectComponent(FrostySdk.Ebx.MaterialBasedEffectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

