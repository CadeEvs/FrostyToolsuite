
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LinkedProxyEntityTrackData))]
	public class LinkedProxyEntityTrack : TemplatedProxyEntityTrack, IEntityData<FrostySdk.Ebx.LinkedProxyEntityTrackData>
	{
		public new FrostySdk.Ebx.LinkedProxyEntityTrackData Data => data as FrostySdk.Ebx.LinkedProxyEntityTrackData;
		public override string DisplayName => "LinkedProxyEntityTrack";

		public LinkedProxyEntityTrack(FrostySdk.Ebx.LinkedProxyEntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

