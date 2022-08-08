using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBPhysicsSubLevelData))]
	public class FBPhysicsSubLevel : LogicEntity, IEntityData<FrostySdk.Ebx.FBPhysicsSubLevelData>
	{
		public new FrostySdk.Ebx.FBPhysicsSubLevelData Data => data as FrostySdk.Ebx.FBPhysicsSubLevelData;
		public override string DisplayName => "FBPhysicsSubLevel";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FBPhysicsSubLevel(FrostySdk.Ebx.FBPhysicsSubLevelData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

