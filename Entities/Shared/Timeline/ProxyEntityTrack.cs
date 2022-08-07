
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyEntityTrackData))]
	public class ProxyEntityTrack : EntityTrackBase, IEntityData<FrostySdk.Ebx.ProxyEntityTrackData>
	{
		public new FrostySdk.Ebx.ProxyEntityTrackData Data => data as FrostySdk.Ebx.ProxyEntityTrackData;
		public override string DisplayName => "ProxyEntityTrack";

		public ProxyEntityTrack(FrostySdk.Ebx.ProxyEntityTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

