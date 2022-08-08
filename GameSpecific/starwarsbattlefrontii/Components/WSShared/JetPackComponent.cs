
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JetPackComponentData))]
	public class JetPackComponent : GameComponent, IEntityData<FrostySdk.Ebx.JetPackComponentData>
	{
		public new FrostySdk.Ebx.JetPackComponentData Data => data as FrostySdk.Ebx.JetPackComponentData;
		public override string DisplayName => "JetPackComponent";

		public JetPackComponent(FrostySdk.Ebx.JetPackComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

