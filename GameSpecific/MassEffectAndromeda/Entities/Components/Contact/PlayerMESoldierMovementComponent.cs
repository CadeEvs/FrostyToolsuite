using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerMESoldierMovementComponentData))]
	public class PlayerMESoldierMovementComponent : MESoldierMovementComponent, IEntityData<FrostySdk.Ebx.PlayerMESoldierMovementComponentData>
	{
		public new FrostySdk.Ebx.PlayerMESoldierMovementComponentData Data => data as FrostySdk.Ebx.PlayerMESoldierMovementComponentData;
		public override string DisplayName => "PlayerMESoldierMovementComponent";

		public PlayerMESoldierMovementComponent(FrostySdk.Ebx.PlayerMESoldierMovementComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

