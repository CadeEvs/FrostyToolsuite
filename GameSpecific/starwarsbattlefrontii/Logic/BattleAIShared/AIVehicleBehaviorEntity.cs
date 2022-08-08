using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIVehicleBehaviorEntityData))]
	public class AIVehicleBehaviorEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIVehicleBehaviorEntityData>
	{
		public new FrostySdk.Ebx.AIVehicleBehaviorEntityData Data => data as FrostySdk.Ebx.AIVehicleBehaviorEntityData;
		public override string DisplayName => "AIVehicleBehavior";

		public AIVehicleBehaviorEntity(FrostySdk.Ebx.AIVehicleBehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

