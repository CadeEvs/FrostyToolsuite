
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HologramParamsComponentData))]
	public class HologramParamsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.HologramParamsComponentData>
	{
		public new FrostySdk.Ebx.HologramParamsComponentData Data => data as FrostySdk.Ebx.HologramParamsComponentData;
		public override string DisplayName => "HologramParamsComponent";

		public HologramParamsComponent(FrostySdk.Ebx.HologramParamsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

