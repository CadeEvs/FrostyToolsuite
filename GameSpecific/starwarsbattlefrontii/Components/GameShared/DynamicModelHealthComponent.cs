
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicModelHealthComponentData))]
	public class DynamicModelHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.DynamicModelHealthComponentData>
	{
		public new FrostySdk.Ebx.DynamicModelHealthComponentData Data => data as FrostySdk.Ebx.DynamicModelHealthComponentData;
		public override string DisplayName => "DynamicModelHealthComponent";

		public DynamicModelHealthComponent(FrostySdk.Ebx.DynamicModelHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

