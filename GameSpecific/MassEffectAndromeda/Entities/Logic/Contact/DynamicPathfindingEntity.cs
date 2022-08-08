using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicPathfindingEntityData))]
	public class DynamicPathfindingEntity : PathfindingEntityBase, IEntityData<FrostySdk.Ebx.DynamicPathfindingEntityData>
	{
		public new FrostySdk.Ebx.DynamicPathfindingEntityData Data => data as FrostySdk.Ebx.DynamicPathfindingEntityData;
		public override string DisplayName => "DynamicPathfinding";

		public DynamicPathfindingEntity(FrostySdk.Ebx.DynamicPathfindingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

