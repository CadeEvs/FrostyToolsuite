
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpottingComponentData))]
	public class SpottingComponent : GameComponent, IEntityData<FrostySdk.Ebx.SpottingComponentData>
	{
		public new FrostySdk.Ebx.SpottingComponentData Data => data as FrostySdk.Ebx.SpottingComponentData;
		public override string DisplayName => "SpottingComponent";

		public SpottingComponent(FrostySdk.Ebx.SpottingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

