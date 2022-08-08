
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LensFlareComponentData))]
	public class LensFlareComponent : GameComponent, IEntityData<FrostySdk.Ebx.LensFlareComponentData>
	{
		public new FrostySdk.Ebx.LensFlareComponentData Data => data as FrostySdk.Ebx.LensFlareComponentData;
		public override string DisplayName => "LensFlareComponent";

		public LensFlareComponent(FrostySdk.Ebx.LensFlareComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

