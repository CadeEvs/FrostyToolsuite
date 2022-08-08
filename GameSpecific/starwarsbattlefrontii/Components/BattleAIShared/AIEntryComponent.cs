
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIEntryComponentData))]
	public class AIEntryComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIEntryComponentData>
	{
		public new FrostySdk.Ebx.AIEntryComponentData Data => data as FrostySdk.Ebx.AIEntryComponentData;
		public override string DisplayName => "AIEntryComponent";

		public AIEntryComponent(FrostySdk.Ebx.AIEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

