using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleEntryComponentData))]
	public class VehicleEntryComponent : PlayerEntryComponent, IEntityData<FrostySdk.Ebx.VehicleEntryComponentData>
	{
		public new FrostySdk.Ebx.VehicleEntryComponentData Data => data as FrostySdk.Ebx.VehicleEntryComponentData;
		public override string DisplayName => "VehicleEntryComponent";

		public VehicleEntryComponent(FrostySdk.Ebx.VehicleEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

