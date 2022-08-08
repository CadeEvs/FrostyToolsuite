
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierHealthComponentData))]
	public class SoldierHealthComponent : CharacterHealthComponent, IEntityData<FrostySdk.Ebx.SoldierHealthComponentData>
	{
		public new FrostySdk.Ebx.SoldierHealthComponentData Data => data as FrostySdk.Ebx.SoldierHealthComponentData;
		public override string DisplayName => "SoldierHealthComponent";

		public SoldierHealthComponent(FrostySdk.Ebx.SoldierHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

