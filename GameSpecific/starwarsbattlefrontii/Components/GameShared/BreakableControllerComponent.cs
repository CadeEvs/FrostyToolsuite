
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BreakableControllerComponentData))]
	public class BreakableControllerComponent : DestructionControllerComponent, IEntityData<FrostySdk.Ebx.BreakableControllerComponentData>
	{
		public new FrostySdk.Ebx.BreakableControllerComponentData Data => data as FrostySdk.Ebx.BreakableControllerComponentData;
		public override string DisplayName => "BreakableControllerComponent";

		public BreakableControllerComponent(FrostySdk.Ebx.BreakableControllerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

