
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathFollowingComponentBaseData))]
	public class PathFollowingComponentBase : GameComponent, IEntityData<FrostySdk.Ebx.PathFollowingComponentBaseData>
	{
		public new FrostySdk.Ebx.PathFollowingComponentBaseData Data => data as FrostySdk.Ebx.PathFollowingComponentBaseData;
		public override string DisplayName => "PathFollowingComponentBase";

		public PathFollowingComponentBase(FrostySdk.Ebx.PathFollowingComponentBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

