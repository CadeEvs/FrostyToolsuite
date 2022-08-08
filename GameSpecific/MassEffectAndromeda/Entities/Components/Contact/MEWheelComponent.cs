using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEWheelComponentData))]
	public class MEWheelComponent : WheelComponent, IEntityData<FrostySdk.Ebx.MEWheelComponentData>
	{
		public new FrostySdk.Ebx.MEWheelComponentData Data => data as FrostySdk.Ebx.MEWheelComponentData;
		public override string DisplayName => "MEWheelComponent";

		public MEWheelComponent(FrostySdk.Ebx.MEWheelComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

