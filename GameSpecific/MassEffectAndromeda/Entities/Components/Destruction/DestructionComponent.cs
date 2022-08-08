using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestructionComponentData))]
	public class DestructionComponent : Component, IEntityData<FrostySdk.Ebx.DestructionComponentData>
	{
		public new FrostySdk.Ebx.DestructionComponentData Data => data as FrostySdk.Ebx.DestructionComponentData;
		public override string DisplayName => "DestructionComponent";

		public DestructionComponent(FrostySdk.Ebx.DestructionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

