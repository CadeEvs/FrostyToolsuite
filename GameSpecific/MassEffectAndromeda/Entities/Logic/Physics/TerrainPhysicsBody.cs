using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainPhysicsBodyData))]
	public class TerrainPhysicsBody : PhysicsBody, IEntityData<FrostySdk.Ebx.TerrainPhysicsBodyData>
	{
		public new FrostySdk.Ebx.TerrainPhysicsBodyData Data => data as FrostySdk.Ebx.TerrainPhysicsBodyData;
		public override string DisplayName => "TerrainPhysicsBody";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TerrainPhysicsBody(FrostySdk.Ebx.TerrainPhysicsBodyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

