
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierSuppressionComponentData))]
	public class SoldierSuppressionComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierSuppressionComponentData>
	{
		public new FrostySdk.Ebx.SoldierSuppressionComponentData Data => data as FrostySdk.Ebx.SoldierSuppressionComponentData;
		public override string DisplayName => "SoldierSuppressionComponent";

		public SoldierSuppressionComponent(FrostySdk.Ebx.SoldierSuppressionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

