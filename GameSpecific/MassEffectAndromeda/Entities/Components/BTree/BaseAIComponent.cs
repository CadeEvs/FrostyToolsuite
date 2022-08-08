using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BaseAIComponentData))]
	public class BaseAIComponent : GameComponent, IEntityData<FrostySdk.Ebx.BaseAIComponentData>
	{
		public new FrostySdk.Ebx.BaseAIComponentData Data => data as FrostySdk.Ebx.BaseAIComponentData;
		public override string DisplayName => "BaseAIComponent";

		public BaseAIComponent(FrostySdk.Ebx.BaseAIComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

