
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISpottingComponentData))]
	public class AISpottingComponent : GameComponent, IEntityData<FrostySdk.Ebx.AISpottingComponentData>
	{
		public new FrostySdk.Ebx.AISpottingComponentData Data => data as FrostySdk.Ebx.AISpottingComponentData;
		public override string DisplayName => "AISpottingComponent";

		public AISpottingComponent(FrostySdk.Ebx.AISpottingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

