using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientAIComponentData))]
	public class AmbientAIComponent : GameComponent, IEntityData<FrostySdk.Ebx.AmbientAIComponentData>
	{
		public new FrostySdk.Ebx.AmbientAIComponentData Data => data as FrostySdk.Ebx.AmbientAIComponentData;
		public override string DisplayName => "AmbientAIComponent";

		public AmbientAIComponent(FrostySdk.Ebx.AmbientAIComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

