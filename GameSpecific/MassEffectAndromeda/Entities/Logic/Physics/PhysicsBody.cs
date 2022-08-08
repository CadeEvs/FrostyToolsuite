using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsBodyData))]
	public class PhysicsBody : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsBodyData>
	{
		public new FrostySdk.Ebx.PhysicsBodyData Data => data as FrostySdk.Ebx.PhysicsBodyData;
		public override string DisplayName => "PhysicsBody";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsBody(FrostySdk.Ebx.PhysicsBodyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

