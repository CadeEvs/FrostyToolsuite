using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathfindingLoadAgentComponentData))]
	public class PathfindingLoadAgentComponent : GameComponent, IEntityData<FrostySdk.Ebx.PathfindingLoadAgentComponentData>
	{
		public new FrostySdk.Ebx.PathfindingLoadAgentComponentData Data => data as FrostySdk.Ebx.PathfindingLoadAgentComponentData;
		public override string DisplayName => "PathfindingLoadAgentComponent";

		public PathfindingLoadAgentComponent(FrostySdk.Ebx.PathfindingLoadAgentComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

