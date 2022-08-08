using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationComponentData))]
	public class ConversationComponent : GameComponent, IEntityData<FrostySdk.Ebx.ConversationComponentData>
	{
		public new FrostySdk.Ebx.ConversationComponentData Data => data as FrostySdk.Ebx.ConversationComponentData;
		public override string DisplayName => "ConversationComponent";

		public ConversationComponent(FrostySdk.Ebx.ConversationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

