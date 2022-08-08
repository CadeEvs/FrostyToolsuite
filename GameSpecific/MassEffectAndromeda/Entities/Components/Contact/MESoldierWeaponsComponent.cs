using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESoldierWeaponsComponentData))]
	public class MESoldierWeaponsComponent : GameComponent, IEntityData<FrostySdk.Ebx.MESoldierWeaponsComponentData>
	{
		public new FrostySdk.Ebx.MESoldierWeaponsComponentData Data => data as FrostySdk.Ebx.MESoldierWeaponsComponentData;
		public override string DisplayName => "MESoldierWeaponsComponent";

		public MESoldierWeaponsComponent(FrostySdk.Ebx.MESoldierWeaponsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

