
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestructionControllerComponentData))]
	public class DestructionControllerComponent : GameComponent, IEntityData<FrostySdk.Ebx.DestructionControllerComponentData>
	{
		public new FrostySdk.Ebx.DestructionControllerComponentData Data => data as FrostySdk.Ebx.DestructionControllerComponentData;
		public override string DisplayName => "DestructionControllerComponent";

		public DestructionControllerComponent(FrostySdk.Ebx.DestructionControllerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

