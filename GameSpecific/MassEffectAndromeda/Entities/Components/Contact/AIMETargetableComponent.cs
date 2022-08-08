using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIMETargetableComponentData))]
	public class AIMETargetableComponent : METargetableComponent, IEntityData<FrostySdk.Ebx.AIMETargetableComponentData>
	{
		public new FrostySdk.Ebx.AIMETargetableComponentData Data => data as FrostySdk.Ebx.AIMETargetableComponentData;
		public override string DisplayName => "AIMETargetableComponent";

		public AIMETargetableComponent(FrostySdk.Ebx.AIMETargetableComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

