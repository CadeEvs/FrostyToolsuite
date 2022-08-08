
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierSteeringComponentData))]
	public class SoldierSteeringComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierSteeringComponentData>
	{
		public new FrostySdk.Ebx.SoldierSteeringComponentData Data => data as FrostySdk.Ebx.SoldierSteeringComponentData;
		public override string DisplayName => "SoldierSteeringComponent";

		public SoldierSteeringComponent(FrostySdk.Ebx.SoldierSteeringComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

