
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierWeaponsPreviewComponentData))]
	public class SoldierWeaponsPreviewComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierWeaponsPreviewComponentData>
	{
		public new FrostySdk.Ebx.SoldierWeaponsPreviewComponentData Data => data as FrostySdk.Ebx.SoldierWeaponsPreviewComponentData;
		public override string DisplayName => "SoldierWeaponsPreviewComponent";

		public SoldierWeaponsPreviewComponent(FrostySdk.Ebx.SoldierWeaponsPreviewComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

