
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnyPartyMemberProxyObjectTrackData))]
	public class AnyPartyMemberProxyObjectTrack : PartyMemberProxyObjectTrack, IEntityData<FrostySdk.Ebx.AnyPartyMemberProxyObjectTrackData>
	{
		public new FrostySdk.Ebx.AnyPartyMemberProxyObjectTrackData Data => data as FrostySdk.Ebx.AnyPartyMemberProxyObjectTrackData;
		public override string DisplayName => "AnyPartyMemberProxyObjectTrack";

		public AnyPartyMemberProxyObjectTrack(FrostySdk.Ebx.AnyPartyMemberProxyObjectTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

