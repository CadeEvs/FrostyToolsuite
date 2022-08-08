using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathfindingStreamEntityBaseData))]
	public class PathfindingStreamEntityBase : PathfindingEntityBase, IEntityData<FrostySdk.Ebx.PathfindingStreamEntityBaseData>
	{
		public new FrostySdk.Ebx.PathfindingStreamEntityBaseData Data => data as FrostySdk.Ebx.PathfindingStreamEntityBaseData;
		public override string DisplayName => "PathfindingStreamEntityBase";

		public PathfindingStreamEntityBase(FrostySdk.Ebx.PathfindingStreamEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

