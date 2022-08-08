using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAppearanceComponentData))]
	public class MEAppearanceComponent : AppearanceComponent, IEntityData<FrostySdk.Ebx.MEAppearanceComponentData>
	{
		public new FrostySdk.Ebx.MEAppearanceComponentData Data => data as FrostySdk.Ebx.MEAppearanceComponentData;
		public override string DisplayName => "MEAppearanceComponent";

		public MEAppearanceComponent(FrostySdk.Ebx.MEAppearanceComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

