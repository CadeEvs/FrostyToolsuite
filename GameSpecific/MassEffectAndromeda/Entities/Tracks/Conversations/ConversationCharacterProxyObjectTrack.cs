
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationCharacterProxyObjectTrackData))]
	public class ConversationCharacterProxyObjectTrack : PartyMemberProxyObjectTrack, IEntityData<FrostySdk.Ebx.ConversationCharacterProxyObjectTrackData>
	{
		public new FrostySdk.Ebx.ConversationCharacterProxyObjectTrackData Data => data as FrostySdk.Ebx.ConversationCharacterProxyObjectTrackData;
		public override string Icon => "Images/Tracks/ConversationTrack.png";

        public ConversationCharacterProxyObjectTrack(FrostySdk.Ebx.ConversationCharacterProxyObjectTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			trackName = FrostySdk.Utils.GetString(Data.CharacterLinkId);
		}
    }
}

