using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestructionPartComponentData))]
	public class DestructionPartComponent : GameComponent, IEntityData<FrostySdk.Ebx.DestructionPartComponentData>
	{
		public new FrostySdk.Ebx.DestructionPartComponentData Data => data as FrostySdk.Ebx.DestructionPartComponentData;
		public override string DisplayName => "DestructionPartComponent";

		public DestructionPartComponent(FrostySdk.Ebx.DestructionPartComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

