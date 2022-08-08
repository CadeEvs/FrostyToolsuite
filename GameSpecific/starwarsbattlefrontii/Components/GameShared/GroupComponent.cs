
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroupComponentData))]
	public class GroupComponent : GameComponent, IEntityData<FrostySdk.Ebx.GroupComponentData>
	{
		public new FrostySdk.Ebx.GroupComponentData Data => data as FrostySdk.Ebx.GroupComponentData;
		public override string DisplayName => "GroupComponent";

		public GroupComponent(FrostySdk.Ebx.GroupComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

