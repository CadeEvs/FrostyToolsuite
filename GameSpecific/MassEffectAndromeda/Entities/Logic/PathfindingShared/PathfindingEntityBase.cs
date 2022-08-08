using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathfindingEntityBaseData))]
	public class PathfindingEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.PathfindingEntityBaseData>
	{
		public new FrostySdk.Ebx.PathfindingEntityBaseData Data => data as FrostySdk.Ebx.PathfindingEntityBaseData;
		public override string DisplayName => "PathfindingEntityBase";

		public PathfindingEntityBase(FrostySdk.Ebx.PathfindingEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

