
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterPhysicsComponentData))]
	public class WaterPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.WaterPhysicsComponentData>
	{
		public new FrostySdk.Ebx.WaterPhysicsComponentData Data => data as FrostySdk.Ebx.WaterPhysicsComponentData;
		public override string DisplayName => "WaterPhysicsComponent";

		public WaterPhysicsComponent(FrostySdk.Ebx.WaterPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

