using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EffectAttachmentComponentData))]
	public class EffectAttachmentComponent : Component, IEntityData<FrostySdk.Ebx.EffectAttachmentComponentData>
	{
		public new FrostySdk.Ebx.EffectAttachmentComponentData Data => data as FrostySdk.Ebx.EffectAttachmentComponentData;
		public override string DisplayName => "EffectAttachmentComponent";

		public EffectAttachmentComponent(FrostySdk.Ebx.EffectAttachmentComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

