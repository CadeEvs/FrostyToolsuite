using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AabbTriggerPhysicsBodyData))]
	public class AabbTriggerPhysicsBody : PhysicsBody, IEntityData<FrostySdk.Ebx.AabbTriggerPhysicsBodyData>
	{
		public new FrostySdk.Ebx.AabbTriggerPhysicsBodyData Data => data as FrostySdk.Ebx.AabbTriggerPhysicsBodyData;
		public override string DisplayName => "AabbTriggerPhysicsBody";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AabbTriggerPhysicsBody(FrostySdk.Ebx.AabbTriggerPhysicsBodyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

