using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerMESoldierWeaponsComponentData))]
	public class PlayerMESoldierWeaponsComponent : MESoldierWeaponsComponent, IEntityData<FrostySdk.Ebx.PlayerMESoldierWeaponsComponentData>
	{
		public new FrostySdk.Ebx.PlayerMESoldierWeaponsComponentData Data => data as FrostySdk.Ebx.PlayerMESoldierWeaponsComponentData;
		public override string DisplayName => "PlayerMESoldierWeaponsComponent";

		public PlayerMESoldierWeaponsComponent(FrostySdk.Ebx.PlayerMESoldierWeaponsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

