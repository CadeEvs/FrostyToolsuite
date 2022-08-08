
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IKComponentData))]
	public class IKComponent : GameComponent, IEntityData<FrostySdk.Ebx.IKComponentData>
	{
		public new FrostySdk.Ebx.IKComponentData Data => data as FrostySdk.Ebx.IKComponentData;
		public override string DisplayName => "IKComponent";

		public IKComponent(FrostySdk.Ebx.IKComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

