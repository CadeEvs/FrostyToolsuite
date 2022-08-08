
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleComponentData))]
	public class VehicleComponent : ChassisComponent, IEntityData<FrostySdk.Ebx.VehicleComponentData>
	{
		public new FrostySdk.Ebx.VehicleComponentData Data => data as FrostySdk.Ebx.VehicleComponentData;
		public override string DisplayName => "VehicleComponent";

		public VehicleComponent(FrostySdk.Ebx.VehicleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

