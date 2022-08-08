using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.METargetableComponentData))]
	public class METargetableComponent : BWTargetComponent, IEntityData<FrostySdk.Ebx.METargetableComponentData>
	{
		public new FrostySdk.Ebx.METargetableComponentData Data => data as FrostySdk.Ebx.METargetableComponentData;
		public override string DisplayName => "METargetableComponent";

		public METargetableComponent(FrostySdk.Ebx.METargetableComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

