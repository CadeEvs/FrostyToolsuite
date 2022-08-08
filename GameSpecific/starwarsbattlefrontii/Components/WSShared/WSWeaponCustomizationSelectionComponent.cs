
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSWeaponCustomizationSelectionComponentData))]
	public class WSWeaponCustomizationSelectionComponent : GameComponent, IEntityData<FrostySdk.Ebx.WSWeaponCustomizationSelectionComponentData>
	{
		public new FrostySdk.Ebx.WSWeaponCustomizationSelectionComponentData Data => data as FrostySdk.Ebx.WSWeaponCustomizationSelectionComponentData;
		public override string DisplayName => "WSWeaponCustomizationSelectionComponent";

		public WSWeaponCustomizationSelectionComponent(FrostySdk.Ebx.WSWeaponCustomizationSelectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

