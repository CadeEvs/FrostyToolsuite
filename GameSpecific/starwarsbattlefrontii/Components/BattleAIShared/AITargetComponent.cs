
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AITargetComponentData))]
	public class AITargetComponent : GameComponent, IEntityData<FrostySdk.Ebx.AITargetComponentData>
	{
		public new FrostySdk.Ebx.AITargetComponentData Data => data as FrostySdk.Ebx.AITargetComponentData;
		public override string DisplayName => "AITargetComponent";

		public AITargetComponent(FrostySdk.Ebx.AITargetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

