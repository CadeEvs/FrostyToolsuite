
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterHitComponentData))]
	public class WaterHitComponent : GameComponent, IEntityData<FrostySdk.Ebx.WaterHitComponentData>
	{
		public new FrostySdk.Ebx.WaterHitComponentData Data => data as FrostySdk.Ebx.WaterHitComponentData;
		public override string DisplayName => "WaterHitComponent";

		public WaterHitComponent(FrostySdk.Ebx.WaterHitComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

