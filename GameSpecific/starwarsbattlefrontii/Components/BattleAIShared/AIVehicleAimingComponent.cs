
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIVehicleAimingComponentData))]
	public class AIVehicleAimingComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIVehicleAimingComponentData>
	{
		public new FrostySdk.Ebx.AIVehicleAimingComponentData Data => data as FrostySdk.Ebx.AIVehicleAimingComponentData;
		public override string DisplayName => "AIVehicleAimingComponent";

		public AIVehicleAimingComponent(FrostySdk.Ebx.AIVehicleAimingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

