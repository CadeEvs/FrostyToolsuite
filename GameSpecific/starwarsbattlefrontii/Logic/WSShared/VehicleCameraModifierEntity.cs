using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleCameraModifierEntityData))]
	public class VehicleCameraModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VehicleCameraModifierEntityData>
	{
		public new FrostySdk.Ebx.VehicleCameraModifierEntityData Data => data as FrostySdk.Ebx.VehicleCameraModifierEntityData;
		public override string DisplayName => "VehicleCameraModifier";

		public VehicleCameraModifierEntity(FrostySdk.Ebx.VehicleCameraModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

