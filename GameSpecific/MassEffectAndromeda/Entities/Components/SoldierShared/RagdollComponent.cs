using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RagdollComponentData))]
	public class RagdollComponent : GameComponent, IEntityData<FrostySdk.Ebx.RagdollComponentData>
	{
		public new FrostySdk.Ebx.RagdollComponentData Data => data as FrostySdk.Ebx.RagdollComponentData;
		public override string DisplayName => "RagdollComponent";

		public RagdollComponent(FrostySdk.Ebx.RagdollComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

