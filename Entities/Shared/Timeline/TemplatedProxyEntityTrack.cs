
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TemplatedProxyEntityTrackData))]
	public class TemplatedProxyEntityTrack : ProxyEntityTrack, IEntityData<FrostySdk.Ebx.TemplatedProxyEntityTrackData>
	{
		public new FrostySdk.Ebx.TemplatedProxyEntityTrackData Data => data as FrostySdk.Ebx.TemplatedProxyEntityTrackData;
		public override string DisplayName => "TemplatedProxyEntityTrack";

		public TemplatedProxyEntityTrack(FrostySdk.Ebx.TemplatedProxyEntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

