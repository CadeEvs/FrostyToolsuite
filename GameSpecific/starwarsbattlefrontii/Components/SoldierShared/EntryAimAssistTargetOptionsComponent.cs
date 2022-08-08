
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntryAimAssistTargetOptionsComponentData))]
	public class EntryAimAssistTargetOptionsComponent : GameComponent, IEntityData<FrostySdk.Ebx.EntryAimAssistTargetOptionsComponentData>
	{
		public new FrostySdk.Ebx.EntryAimAssistTargetOptionsComponentData Data => data as FrostySdk.Ebx.EntryAimAssistTargetOptionsComponentData;
		public override string DisplayName => "EntryAimAssistTargetOptionsComponent";

		public EntryAimAssistTargetOptionsComponent(FrostySdk.Ebx.EntryAimAssistTargetOptionsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

