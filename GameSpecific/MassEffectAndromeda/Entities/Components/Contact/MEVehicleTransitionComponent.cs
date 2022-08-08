using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEVehicleTransitionComponentData))]
	public class MEVehicleTransitionComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEVehicleTransitionComponentData>
	{
		public new FrostySdk.Ebx.MEVehicleTransitionComponentData Data => data as FrostySdk.Ebx.MEVehicleTransitionComponentData;
		public override string DisplayName => "MEVehicleTransitionComponent";

		public MEVehicleTransitionComponent(FrostySdk.Ebx.MEVehicleTransitionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

