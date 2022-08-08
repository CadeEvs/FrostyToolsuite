using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerSystemNavigationComponentData))]
	public class PlayerSystemNavigationComponent : GameComponent, IEntityData<FrostySdk.Ebx.PlayerSystemNavigationComponentData>
	{
		public new FrostySdk.Ebx.PlayerSystemNavigationComponentData Data => data as FrostySdk.Ebx.PlayerSystemNavigationComponentData;
		public override string DisplayName => "PlayerSystemNavigationComponent";

		public PlayerSystemNavigationComponent(FrostySdk.Ebx.PlayerSystemNavigationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

