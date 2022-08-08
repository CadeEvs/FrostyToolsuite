
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterDepthComponentData))]
	public class WaterDepthComponent : GameComponent, IEntityData<FrostySdk.Ebx.WaterDepthComponentData>
	{
		public new FrostySdk.Ebx.WaterDepthComponentData Data => data as FrostySdk.Ebx.WaterDepthComponentData;
		public override string DisplayName => "WaterDepthComponent";

		public WaterDepthComponent(FrostySdk.Ebx.WaterDepthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

