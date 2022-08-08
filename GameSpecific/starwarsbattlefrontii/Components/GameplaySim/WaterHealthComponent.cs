
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterHealthComponentData))]
	public class WaterHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.WaterHealthComponentData>
	{
		public new FrostySdk.Ebx.WaterHealthComponentData Data => data as FrostySdk.Ebx.WaterHealthComponentData;
		public override string DisplayName => "WaterHealthComponent";

		public WaterHealthComponent(FrostySdk.Ebx.WaterHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

