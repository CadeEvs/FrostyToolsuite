using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEBreakableArmorComponentData))]
	public class MEBreakableArmorComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEBreakableArmorComponentData>
	{
		public new FrostySdk.Ebx.MEBreakableArmorComponentData Data => data as FrostySdk.Ebx.MEBreakableArmorComponentData;
		public override string DisplayName => "MEBreakableArmorComponent";

		public MEBreakableArmorComponent(FrostySdk.Ebx.MEBreakableArmorComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

