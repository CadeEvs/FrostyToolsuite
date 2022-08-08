using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimAssistNodeComponentData))]
	public class AimAssistNodeComponent : GameComponent, IEntityData<FrostySdk.Ebx.AimAssistNodeComponentData>
	{
		public new FrostySdk.Ebx.AimAssistNodeComponentData Data => data as FrostySdk.Ebx.AimAssistNodeComponentData;
		public override string DisplayName => "AimAssistNodeComponent";

		public AimAssistNodeComponent(FrostySdk.Ebx.AimAssistNodeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

