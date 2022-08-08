using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AirCollisionAvoidanceEntityData))]
	public class AirCollisionAvoidanceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AirCollisionAvoidanceEntityData>
	{
		public new FrostySdk.Ebx.AirCollisionAvoidanceEntityData Data => data as FrostySdk.Ebx.AirCollisionAvoidanceEntityData;
		public override string DisplayName => "AirCollisionAvoidance";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AirCollisionAvoidanceEntity(FrostySdk.Ebx.AirCollisionAvoidanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

