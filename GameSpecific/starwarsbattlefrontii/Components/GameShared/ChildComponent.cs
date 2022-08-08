
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChildComponentData))]
	public class ChildComponent : BoneComponent, IEntityData<FrostySdk.Ebx.ChildComponentData>
	{
		public new FrostySdk.Ebx.ChildComponentData Data => data as FrostySdk.Ebx.ChildComponentData;
		public override string DisplayName => "ChildComponent";

		public ChildComponent(FrostySdk.Ebx.ChildComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

