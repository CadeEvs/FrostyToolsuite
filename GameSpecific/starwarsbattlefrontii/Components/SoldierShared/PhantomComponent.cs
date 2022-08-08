
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhantomComponentData))]
	public class PhantomComponent : GameComponent, IEntityData<FrostySdk.Ebx.PhantomComponentData>
	{
		public new FrostySdk.Ebx.PhantomComponentData Data => data as FrostySdk.Ebx.PhantomComponentData;
		public override string DisplayName => "PhantomComponent";

		public PhantomComponent(FrostySdk.Ebx.PhantomComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

