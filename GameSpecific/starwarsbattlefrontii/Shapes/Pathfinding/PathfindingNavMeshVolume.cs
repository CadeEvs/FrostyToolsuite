using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathfindingNavMeshVolumeData))]
	public class PathfindingNavMeshVolume : OBB, IEntityData<FrostySdk.Ebx.PathfindingNavMeshVolumeData>
	{
		public new FrostySdk.Ebx.PathfindingNavMeshVolumeData Data => data as FrostySdk.Ebx.PathfindingNavMeshVolumeData;
		public override string DisplayName => "PathfindingNavMeshVolume";

		public PathfindingNavMeshVolume(FrostySdk.Ebx.PathfindingNavMeshVolumeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

