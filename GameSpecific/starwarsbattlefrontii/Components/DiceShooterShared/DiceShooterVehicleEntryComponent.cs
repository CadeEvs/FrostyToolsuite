
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceShooterVehicleEntryComponentData))]
	public class DiceShooterVehicleEntryComponent : VehicleEntryComponent, IEntityData<FrostySdk.Ebx.DiceShooterVehicleEntryComponentData>
	{
		public new FrostySdk.Ebx.DiceShooterVehicleEntryComponentData Data => data as FrostySdk.Ebx.DiceShooterVehicleEntryComponentData;
		public override string DisplayName => "DiceShooterVehicleEntryComponent";

		public DiceShooterVehicleEntryComponent(FrostySdk.Ebx.DiceShooterVehicleEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

