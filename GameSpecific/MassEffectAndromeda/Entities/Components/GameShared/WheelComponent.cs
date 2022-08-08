using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WheelComponentData))]
	public class WheelComponent : BoneComponent, IEntityData<FrostySdk.Ebx.WheelComponentData>
	{
		public new FrostySdk.Ebx.WheelComponentData Data => data as FrostySdk.Ebx.WheelComponentData;
		public override string DisplayName => "WheelComponent";

		public WheelComponent(FrostySdk.Ebx.WheelComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

