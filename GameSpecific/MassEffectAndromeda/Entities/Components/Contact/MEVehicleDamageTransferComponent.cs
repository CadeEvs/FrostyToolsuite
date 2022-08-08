using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEVehicleDamageTransferComponentData))]
	public class MEVehicleDamageTransferComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEVehicleDamageTransferComponentData>
	{
		public new FrostySdk.Ebx.MEVehicleDamageTransferComponentData Data => data as FrostySdk.Ebx.MEVehicleDamageTransferComponentData;
		public override string DisplayName => "MEVehicleDamageTransferComponent";

		public MEVehicleDamageTransferComponent(FrostySdk.Ebx.MEVehicleDamageTransferComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

