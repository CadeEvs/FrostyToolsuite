
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameEntryComponentData))]
	public class GameEntryComponent : EntryComponent, IEntityData<FrostySdk.Ebx.GameEntryComponentData>
	{
		public new FrostySdk.Ebx.GameEntryComponentData Data => data as FrostySdk.Ebx.GameEntryComponentData;
		public override string DisplayName => "GameEntryComponent";

		public GameEntryComponent(FrostySdk.Ebx.GameEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

