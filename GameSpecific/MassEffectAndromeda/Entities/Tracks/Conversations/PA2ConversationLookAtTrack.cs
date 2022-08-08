
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2ConversationLookAtTrackData))]
	public class PA2ConversationLookAtTrack : PA2LookAtTrack, IEntityData<FrostySdk.Ebx.PA2ConversationLookAtTrackData>
	{
		public new FrostySdk.Ebx.PA2ConversationLookAtTrackData Data => data as FrostySdk.Ebx.PA2ConversationLookAtTrackData;
		public override string DisplayName => "PA2ConversationLookAtTrack";

		public PA2ConversationLookAtTrack(FrostySdk.Ebx.PA2ConversationLookAtTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

