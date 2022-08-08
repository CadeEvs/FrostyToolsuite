using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MaterialGridEffectsComponentData))]
	public class MaterialGridEffectsComponent : GameComponent, IEntityData<FrostySdk.Ebx.MaterialGridEffectsComponentData>
	{
		public new FrostySdk.Ebx.MaterialGridEffectsComponentData Data => data as FrostySdk.Ebx.MaterialGridEffectsComponentData;
		public override string DisplayName => "MaterialGridEffectsComponent";

		public MaterialGridEffectsComponent(FrostySdk.Ebx.MaterialGridEffectsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

