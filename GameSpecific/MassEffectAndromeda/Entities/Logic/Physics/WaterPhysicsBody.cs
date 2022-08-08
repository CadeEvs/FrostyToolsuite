using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterPhysicsBodyData))]
	public class WaterPhysicsBody : PhysicsBody, IEntityData<FrostySdk.Ebx.WaterPhysicsBodyData>
	{
		public new FrostySdk.Ebx.WaterPhysicsBodyData Data => data as FrostySdk.Ebx.WaterPhysicsBodyData;
		public override string DisplayName => "WaterPhysicsBody";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WaterPhysicsBody(FrostySdk.Ebx.WaterPhysicsBodyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

