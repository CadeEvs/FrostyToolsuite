using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerMESoldierTargetingComponentData))]
	public class PlayerMESoldierTargetingComponent : MESoldierTargetingComponent, IEntityData<FrostySdk.Ebx.PlayerMESoldierTargetingComponentData>
	{
		public new FrostySdk.Ebx.PlayerMESoldierTargetingComponentData Data => data as FrostySdk.Ebx.PlayerMESoldierTargetingComponentData;
		public override string DisplayName => "PlayerMESoldierTargetingComponent";

		public PlayerMESoldierTargetingComponent(FrostySdk.Ebx.PlayerMESoldierTargetingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

