
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientLocalPlayerProxyEntityTrackData))]
	public class ClientLocalPlayerProxyEntityTrack : TemplatedProxyEntityTrack, IEntityData<FrostySdk.Ebx.ClientLocalPlayerProxyEntityTrackData>
	{
		public new FrostySdk.Ebx.ClientLocalPlayerProxyEntityTrackData Data => data as FrostySdk.Ebx.ClientLocalPlayerProxyEntityTrackData;
		public override string DisplayName => "ClientLocalPlayerProxyEntityTrack";

		public ClientLocalPlayerProxyEntityTrack(FrostySdk.Ebx.ClientLocalPlayerProxyEntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

