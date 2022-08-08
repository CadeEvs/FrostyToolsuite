
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSoldierCustomizationComponentData))]
	public class WSSoldierCustomizationComponent : SoldierCustomizationComponent, IEntityData<FrostySdk.Ebx.WSSoldierCustomizationComponentData>
	{
		public new FrostySdk.Ebx.WSSoldierCustomizationComponentData Data => data as FrostySdk.Ebx.WSSoldierCustomizationComponentData;
		public override string DisplayName => "WSSoldierCustomizationComponent";

		public WSSoldierCustomizationComponent(FrostySdk.Ebx.WSSoldierCustomizationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

