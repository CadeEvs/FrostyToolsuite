using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathfindingSystemEntityData))]
	public class PathfindingSystemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PathfindingSystemEntityData>
	{
		public new FrostySdk.Ebx.PathfindingSystemEntityData Data => data as FrostySdk.Ebx.PathfindingSystemEntityData;
		public override string DisplayName => "PathfindingSystem";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PathfindingSystemEntity(FrostySdk.Ebx.PathfindingSystemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

