
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierCustomizationComponentData))]
	public class SoldierCustomizationComponent : CharacterCustomizationComponent, IEntityData<FrostySdk.Ebx.SoldierCustomizationComponentData>
	{
		public new FrostySdk.Ebx.SoldierCustomizationComponentData Data => data as FrostySdk.Ebx.SoldierCustomizationComponentData;
		public override string DisplayName => "SoldierCustomizationComponent";

		public SoldierCustomizationComponent(FrostySdk.Ebx.SoldierCustomizationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

