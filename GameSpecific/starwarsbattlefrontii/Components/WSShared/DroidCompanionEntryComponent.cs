
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionEntryComponentData))]
	public class DroidCompanionEntryComponent : GameEntryComponent, IEntityData<FrostySdk.Ebx.DroidCompanionEntryComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionEntryComponentData Data => data as FrostySdk.Ebx.DroidCompanionEntryComponentData;
		public override string DisplayName => "DroidCompanionEntryComponent";

		public DroidCompanionEntryComponent(FrostySdk.Ebx.DroidCompanionEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

