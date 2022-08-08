using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEConversationComponentData))]
	public class MEConversationComponent : ConversationComponent, IEntityData<FrostySdk.Ebx.MEConversationComponentData>
	{
		public new FrostySdk.Ebx.MEConversationComponentData Data => data as FrostySdk.Ebx.MEConversationComponentData;
		public override string DisplayName => "MEConversationComponent";

		public MEConversationComponent(FrostySdk.Ebx.MEConversationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

