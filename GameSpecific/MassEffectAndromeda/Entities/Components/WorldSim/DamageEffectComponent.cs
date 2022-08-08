using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageEffectComponentData))]
	public class DamageEffectComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.DamageEffectComponentData>
	{
		public new FrostySdk.Ebx.DamageEffectComponentData Data => data as FrostySdk.Ebx.DamageEffectComponentData;
		public override string DisplayName => "DamageEffectComponent";

		public DamageEffectComponent(FrostySdk.Ebx.DamageEffectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

