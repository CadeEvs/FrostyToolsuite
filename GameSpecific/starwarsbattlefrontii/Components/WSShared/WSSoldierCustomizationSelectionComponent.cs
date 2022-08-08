
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSoldierCustomizationSelectionComponentData))]
	public class WSSoldierCustomizationSelectionComponent : GameComponent, IEntityData<FrostySdk.Ebx.WSSoldierCustomizationSelectionComponentData>
	{
		public new FrostySdk.Ebx.WSSoldierCustomizationSelectionComponentData Data => data as FrostySdk.Ebx.WSSoldierCustomizationSelectionComponentData;
		public override string DisplayName => "WSSoldierCustomizationSelectionComponent";

		public WSSoldierCustomizationSelectionComponent(FrostySdk.Ebx.WSSoldierCustomizationSelectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

