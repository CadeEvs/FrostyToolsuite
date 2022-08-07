
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DummyEntityTrackData))]
	public class DummyEntityTrack : EntityTrackBase, IEntityData<FrostySdk.Ebx.DummyEntityTrackData>
	{
		public new FrostySdk.Ebx.DummyEntityTrackData Data => data as FrostySdk.Ebx.DummyEntityTrackData;
		public override string DisplayName => "DummyEntityTrack";

		public DummyEntityTrack(FrostySdk.Ebx.DummyEntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

