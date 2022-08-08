using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICollisionAvoidanceSetupEntityData))]
	public class AICollisionAvoidanceSetupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AICollisionAvoidanceSetupEntityData>
	{
		public new FrostySdk.Ebx.AICollisionAvoidanceSetupEntityData Data => data as FrostySdk.Ebx.AICollisionAvoidanceSetupEntityData;
		public override string DisplayName => "AICollisionAvoidanceSetup";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AICollisionAvoidanceSetupEntity(FrostySdk.Ebx.AICollisionAvoidanceSetupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

