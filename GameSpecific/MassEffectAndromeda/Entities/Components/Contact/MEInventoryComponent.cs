using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEInventoryComponentData))]
	public class MEInventoryComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEInventoryComponentData>
	{
		public new FrostySdk.Ebx.MEInventoryComponentData Data => data as FrostySdk.Ebx.MEInventoryComponentData;
		public override string DisplayName => "MEInventoryComponent";

		public MEInventoryComponent(FrostySdk.Ebx.MEInventoryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

