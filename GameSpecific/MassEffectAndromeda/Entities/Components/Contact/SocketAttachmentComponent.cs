using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SocketAttachmentComponentData))]
	public class SocketAttachmentComponent : EffectAttachmentComponent, IEntityData<FrostySdk.Ebx.SocketAttachmentComponentData>
	{
		public new FrostySdk.Ebx.SocketAttachmentComponentData Data => data as FrostySdk.Ebx.SocketAttachmentComponentData;
		public override string DisplayName => "SocketAttachmentComponent";

		public SocketAttachmentComponent(FrostySdk.Ebx.SocketAttachmentComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

