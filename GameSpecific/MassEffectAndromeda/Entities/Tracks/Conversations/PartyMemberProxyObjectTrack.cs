
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberProxyObjectTrackData))]
	public class PartyMemberProxyObjectTrack : TemplatedProxyEntityTrack, IEntityData<FrostySdk.Ebx.PartyMemberProxyObjectTrackData>
	{
		public new FrostySdk.Ebx.PartyMemberProxyObjectTrackData Data => data as FrostySdk.Ebx.PartyMemberProxyObjectTrackData;
		public override string DisplayName => "PartyMemberProxyObjectTrack";

		public PartyMemberProxyObjectTrack(FrostySdk.Ebx.PartyMemberProxyObjectTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

