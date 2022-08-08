using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterAvoidanceZoneEntityData))]
	public class StarfighterAvoidanceZoneEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterAvoidanceZoneEntityData>
	{
		public new FrostySdk.Ebx.StarfighterAvoidanceZoneEntityData Data => data as FrostySdk.Ebx.StarfighterAvoidanceZoneEntityData;
		public override string DisplayName => "StarfighterAvoidanceZone";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StarfighterAvoidanceZoneEntity(FrostySdk.Ebx.StarfighterAvoidanceZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

