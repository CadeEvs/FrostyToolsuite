using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LootComponentData))]
	public class LootComponent : GameComponent, IEntityData<FrostySdk.Ebx.LootComponentData>
	{
		public new FrostySdk.Ebx.LootComponentData Data => data as FrostySdk.Ebx.LootComponentData;
		public override string DisplayName => "LootComponent";

		public LootComponent(FrostySdk.Ebx.LootComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

