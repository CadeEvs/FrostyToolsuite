
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSoldierHealthComponentData))]
	public class WSSoldierHealthComponent : SoldierHealthComponent, IEntityData<FrostySdk.Ebx.WSSoldierHealthComponentData>
	{
		public new FrostySdk.Ebx.WSSoldierHealthComponentData Data => data as FrostySdk.Ebx.WSSoldierHealthComponentData;
		public override string DisplayName => "WSSoldierHealthComponent";

		public WSSoldierHealthComponent(FrostySdk.Ebx.WSSoldierHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

