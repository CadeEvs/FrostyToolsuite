using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAmbientAIComponentData))]
	public class MEAmbientAIComponent : AmbientAIComponent, IEntityData<FrostySdk.Ebx.MEAmbientAIComponentData>
	{
		public new FrostySdk.Ebx.MEAmbientAIComponentData Data => data as FrostySdk.Ebx.MEAmbientAIComponentData;
		public override string DisplayName => "MEAmbientAIComponent";

		public MEAmbientAIComponent(FrostySdk.Ebx.MEAmbientAIComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

