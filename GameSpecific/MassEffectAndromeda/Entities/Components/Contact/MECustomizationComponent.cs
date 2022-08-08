using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECustomizationComponentData))]
	public class MECustomizationComponent : GameComponent, IEntityData<FrostySdk.Ebx.MECustomizationComponentData>
	{
		public new FrostySdk.Ebx.MECustomizationComponentData Data => data as FrostySdk.Ebx.MECustomizationComponentData;
		public override string DisplayName => "MECustomizationComponent";

		public MECustomizationComponent(FrostySdk.Ebx.MECustomizationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

