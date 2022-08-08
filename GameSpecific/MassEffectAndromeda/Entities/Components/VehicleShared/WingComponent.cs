using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WingComponentData))]
	public class WingComponent : BoneComponent, IEntityData<FrostySdk.Ebx.WingComponentData>
	{
		public new FrostySdk.Ebx.WingComponentData Data => data as FrostySdk.Ebx.WingComponentData;
		public override string DisplayName => "WingComponent";

		public WingComponent(FrostySdk.Ebx.WingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

