
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehiclePassengerTargetingComponentData))]
	public class VehiclePassengerTargetingComponent : GameComponent, IEntityData<FrostySdk.Ebx.VehiclePassengerTargetingComponentData>
	{
		public new FrostySdk.Ebx.VehiclePassengerTargetingComponentData Data => data as FrostySdk.Ebx.VehiclePassengerTargetingComponentData;
		public override string DisplayName => "VehiclePassengerTargetingComponent";

		public VehiclePassengerTargetingComponent(FrostySdk.Ebx.VehiclePassengerTargetingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

