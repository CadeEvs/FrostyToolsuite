
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSVehicleCustomizationComponentData))]
	public class WSVehicleCustomizationComponent : VehicleCustomizationComponent, IEntityData<FrostySdk.Ebx.WSVehicleCustomizationComponentData>
	{
		public new FrostySdk.Ebx.WSVehicleCustomizationComponentData Data => data as FrostySdk.Ebx.WSVehicleCustomizationComponentData;
		public override string DisplayName => "WSVehicleCustomizationComponent";

		public WSVehicleCustomizationComponent(FrostySdk.Ebx.WSVehicleCustomizationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

