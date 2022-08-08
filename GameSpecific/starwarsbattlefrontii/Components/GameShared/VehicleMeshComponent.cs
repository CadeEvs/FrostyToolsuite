
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleMeshComponentData))]
	public class VehicleMeshComponent : GameComponent, IEntityData<FrostySdk.Ebx.VehicleMeshComponentData>
	{
		public new FrostySdk.Ebx.VehicleMeshComponentData Data => data as FrostySdk.Ebx.VehicleMeshComponentData;
		public override string DisplayName => "VehicleMeshComponent";

		public VehicleMeshComponent(FrostySdk.Ebx.VehicleMeshComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

