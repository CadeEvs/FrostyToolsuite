
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierWeaponsComponentData))]
	public class SoldierWeaponsComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierWeaponsComponentData>
	{
		public new FrostySdk.Ebx.SoldierWeaponsComponentData Data => data as FrostySdk.Ebx.SoldierWeaponsComponentData;
		public override string DisplayName => "SoldierWeaponsComponent";

		public SoldierWeaponsComponent(FrostySdk.Ebx.SoldierWeaponsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

