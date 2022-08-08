
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleHealthComponentData))]
	public class VehicleHealthComponent : ControllableHealthComponent, IEntityData<FrostySdk.Ebx.VehicleHealthComponentData>
	{
		public new FrostySdk.Ebx.VehicleHealthComponentData Data => data as FrostySdk.Ebx.VehicleHealthComponentData;
		public override string DisplayName => "VehicleHealthComponent";

		public VehicleHealthComponent(FrostySdk.Ebx.VehicleHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

