using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InteractableComponentData))]
	public class InteractableComponent : GameComponent, IEntityData<FrostySdk.Ebx.InteractableComponentData>
	{
		public new FrostySdk.Ebx.InteractableComponentData Data => data as FrostySdk.Ebx.InteractableComponentData;
		public override string DisplayName => "InteractableComponent";

		public InteractableComponent(FrostySdk.Ebx.InteractableComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

