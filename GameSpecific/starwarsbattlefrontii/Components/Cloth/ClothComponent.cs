
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClothComponentData))]
	public class ClothComponent : Component, IEntityData<FrostySdk.Ebx.ClothComponentData>
	{
		public new FrostySdk.Ebx.ClothComponentData Data => data as FrostySdk.Ebx.ClothComponentData;
		public override string DisplayName => "ClothComponent";

		public ClothComponent(FrostySdk.Ebx.ClothComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

