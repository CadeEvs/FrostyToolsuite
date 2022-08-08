using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AiBrainComponentData))]
	public class AiBrainComponent : BaseAIComponent, IEntityData<FrostySdk.Ebx.AiBrainComponentData>
	{
		public new FrostySdk.Ebx.AiBrainComponentData Data => data as FrostySdk.Ebx.AiBrainComponentData;
		public override string DisplayName => "AiBrainComponent";

		public AiBrainComponent(FrostySdk.Ebx.AiBrainComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

