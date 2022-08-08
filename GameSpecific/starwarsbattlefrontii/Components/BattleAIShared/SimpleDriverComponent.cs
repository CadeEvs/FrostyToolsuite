
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleDriverComponentData))]
	public class SimpleDriverComponent : GameComponent, IEntityData<FrostySdk.Ebx.SimpleDriverComponentData>
	{
		public new FrostySdk.Ebx.SimpleDriverComponentData Data => data as FrostySdk.Ebx.SimpleDriverComponentData;
		public override string DisplayName => "SimpleDriverComponent";

		public SimpleDriverComponent(FrostySdk.Ebx.SimpleDriverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

