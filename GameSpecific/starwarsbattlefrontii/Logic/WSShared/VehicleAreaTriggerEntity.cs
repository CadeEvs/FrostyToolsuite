using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleAreaTriggerEntityData))]
	public class VehicleAreaTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VehicleAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.VehicleAreaTriggerEntityData Data => data as FrostySdk.Ebx.VehicleAreaTriggerEntityData;
		public override string DisplayName => "VehicleAreaTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VehicleAreaTriggerEntity(FrostySdk.Ebx.VehicleAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

