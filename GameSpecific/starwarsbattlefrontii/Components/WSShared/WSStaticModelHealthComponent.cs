
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSStaticModelHealthComponentData))]
	public class WSStaticModelHealthComponent : StaticModelHealthComponent, IEntityData<FrostySdk.Ebx.WSStaticModelHealthComponentData>
	{
		public new FrostySdk.Ebx.WSStaticModelHealthComponentData Data => data as FrostySdk.Ebx.WSStaticModelHealthComponentData;
		public override string DisplayName => "WSStaticModelHealthComponent";

		public WSStaticModelHealthComponent(FrostySdk.Ebx.WSStaticModelHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

