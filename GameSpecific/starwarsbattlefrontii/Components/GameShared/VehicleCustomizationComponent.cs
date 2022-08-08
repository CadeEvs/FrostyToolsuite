
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleCustomizationComponentData))]
	public class VehicleCustomizationComponent : GameComponent, IEntityData<FrostySdk.Ebx.VehicleCustomizationComponentData>
	{
		public new FrostySdk.Ebx.VehicleCustomizationComponentData Data => data as FrostySdk.Ebx.VehicleCustomizationComponentData;
		public override string DisplayName => "VehicleCustomizationComponent";

		public VehicleCustomizationComponent(FrostySdk.Ebx.VehicleCustomizationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

