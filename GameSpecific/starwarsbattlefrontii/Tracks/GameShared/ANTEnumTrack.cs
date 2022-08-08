
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTEnumTrackData))]
	public class ANTEnumTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.ANTEnumTrackData>
	{
		public new FrostySdk.Ebx.ANTEnumTrackData Data => data as FrostySdk.Ebx.ANTEnumTrackData;
		public override string DisplayName => "ANTEnumTrack";

		public ANTEnumTrack(FrostySdk.Ebx.ANTEnumTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

