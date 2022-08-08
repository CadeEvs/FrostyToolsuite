using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TireTrailVehicleComponentData))]
	public class TireTrailVehicleComponent : GameComponent, IEntityData<FrostySdk.Ebx.TireTrailVehicleComponentData>
	{
		public new FrostySdk.Ebx.TireTrailVehicleComponentData Data => data as FrostySdk.Ebx.TireTrailVehicleComponentData;
		public override string DisplayName => "TireTrailVehicleComponent";

		public TireTrailVehicleComponent(FrostySdk.Ebx.TireTrailVehicleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

