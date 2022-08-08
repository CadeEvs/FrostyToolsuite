
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICustomInputComponentData))]
	public class AICustomInputComponent : GameComponent, IEntityData<FrostySdk.Ebx.AICustomInputComponentData>
	{
		public new FrostySdk.Ebx.AICustomInputComponentData Data => data as FrostySdk.Ebx.AICustomInputComponentData;
		public override string DisplayName => "AICustomInputComponent";

		public AICustomInputComponent(FrostySdk.Ebx.AICustomInputComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

