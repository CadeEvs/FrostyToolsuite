
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlockAimAssistComponentData))]
	public class BlockAimAssistComponent : GameComponent, IEntityData<FrostySdk.Ebx.BlockAimAssistComponentData>
	{
		public new FrostySdk.Ebx.BlockAimAssistComponentData Data => data as FrostySdk.Ebx.BlockAimAssistComponentData;
		public override string DisplayName => "BlockAimAssistComponent";

		public BlockAimAssistComponent(FrostySdk.Ebx.BlockAimAssistComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

