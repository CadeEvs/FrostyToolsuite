
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordTrackChildrenData))]
	public class RecordTrackChildren : RecordTrackBase, IEntityData<FrostySdk.Ebx.RecordTrackChildrenData>
	{
		public new FrostySdk.Ebx.RecordTrackChildrenData Data => data as FrostySdk.Ebx.RecordTrackChildrenData;
		public override string DisplayName => "RecordTrackChildren";

		public RecordTrackChildren(FrostySdk.Ebx.RecordTrackChildrenData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

