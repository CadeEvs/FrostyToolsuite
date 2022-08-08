using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathfindingOverrideData))]
	public class PathfindingOverride : LogicEntity, IEntityData<FrostySdk.Ebx.PathfindingOverrideData>
	{
		public new FrostySdk.Ebx.PathfindingOverrideData Data => data as FrostySdk.Ebx.PathfindingOverrideData;
		public override string DisplayName => "PathfindingOverride";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PathfindingOverride(FrostySdk.Ebx.PathfindingOverrideData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

