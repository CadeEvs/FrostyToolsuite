
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleDroidBehaviorComponentData))]
	public class SimpleDroidBehaviorComponent : GameComponent, IEntityData<FrostySdk.Ebx.SimpleDroidBehaviorComponentData>
	{
		public new FrostySdk.Ebx.SimpleDroidBehaviorComponentData Data => data as FrostySdk.Ebx.SimpleDroidBehaviorComponentData;
		public override string DisplayName => "SimpleDroidBehaviorComponent";

		public SimpleDroidBehaviorComponent(FrostySdk.Ebx.SimpleDroidBehaviorComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

