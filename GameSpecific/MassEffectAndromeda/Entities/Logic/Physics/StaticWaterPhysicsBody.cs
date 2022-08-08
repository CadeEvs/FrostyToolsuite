using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticWaterPhysicsBodyData))]
	public class StaticWaterPhysicsBody : WaterPhysicsBody, IEntityData<FrostySdk.Ebx.StaticWaterPhysicsBodyData>
	{
		public new FrostySdk.Ebx.StaticWaterPhysicsBodyData Data => data as FrostySdk.Ebx.StaticWaterPhysicsBodyData;
		public override string DisplayName => "StaticWaterPhysicsBody";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StaticWaterPhysicsBody(FrostySdk.Ebx.StaticWaterPhysicsBodyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

