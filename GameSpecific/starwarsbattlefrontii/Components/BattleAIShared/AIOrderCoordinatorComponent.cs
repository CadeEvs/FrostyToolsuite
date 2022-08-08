
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIOrderCoordinatorComponentData))]
	public class AIOrderCoordinatorComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIOrderCoordinatorComponentData>
	{
		public new FrostySdk.Ebx.AIOrderCoordinatorComponentData Data => data as FrostySdk.Ebx.AIOrderCoordinatorComponentData;
		public override string DisplayName => "AIOrderCoordinatorComponent";

		public AIOrderCoordinatorComponent(FrostySdk.Ebx.AIOrderCoordinatorComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

