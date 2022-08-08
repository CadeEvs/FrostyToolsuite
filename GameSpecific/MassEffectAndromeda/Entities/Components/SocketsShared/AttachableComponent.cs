using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttachableComponentData))]
	public class AttachableComponent : GameComponent, IEntityData<FrostySdk.Ebx.AttachableComponentData>
	{
		public new FrostySdk.Ebx.AttachableComponentData Data => data as FrostySdk.Ebx.AttachableComponentData;
		public override string DisplayName => "AttachableComponent";

		public AttachableComponent(FrostySdk.Ebx.AttachableComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

