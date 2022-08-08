
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathFollowingComponentData))]
	public class PathFollowingComponent : PathFollowingComponentBase, IEntityData<FrostySdk.Ebx.PathFollowingComponentData>
	{
		public new FrostySdk.Ebx.PathFollowingComponentData Data => data as FrostySdk.Ebx.PathFollowingComponentData;
		public override string DisplayName => "PathFollowingComponent";

		public PathFollowingComponent(FrostySdk.Ebx.PathFollowingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

