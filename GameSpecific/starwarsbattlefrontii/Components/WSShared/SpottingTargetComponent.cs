
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpottingTargetComponentData))]
	public class SpottingTargetComponent : GameComponent, IEntityData<FrostySdk.Ebx.SpottingTargetComponentData>
	{
		public new FrostySdk.Ebx.SpottingTargetComponentData Data => data as FrostySdk.Ebx.SpottingTargetComponentData;
		public override string DisplayName => "SpottingTargetComponent";

		public SpottingTargetComponent(FrostySdk.Ebx.SpottingTargetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

