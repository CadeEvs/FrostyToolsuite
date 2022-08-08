
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIProximityReactionsComponentData))]
	public class AIProximityReactionsComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIProximityReactionsComponentData>
	{
		public new FrostySdk.Ebx.AIProximityReactionsComponentData Data => data as FrostySdk.Ebx.AIProximityReactionsComponentData;
		public override string DisplayName => "AIProximityReactionsComponent";

		public AIProximityReactionsComponent(FrostySdk.Ebx.AIProximityReactionsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

