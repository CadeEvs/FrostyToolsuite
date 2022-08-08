using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GrenadeFeedbackComponentData))]
	public class GrenadeFeedbackComponent : GameComponent, IEntityData<FrostySdk.Ebx.GrenadeFeedbackComponentData>
	{
		public new FrostySdk.Ebx.GrenadeFeedbackComponentData Data => data as FrostySdk.Ebx.GrenadeFeedbackComponentData;
		public override string DisplayName => "GrenadeFeedbackComponent";

		public GrenadeFeedbackComponent(FrostySdk.Ebx.GrenadeFeedbackComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

