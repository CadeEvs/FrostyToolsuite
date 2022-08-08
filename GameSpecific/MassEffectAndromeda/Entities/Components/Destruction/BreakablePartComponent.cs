using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BreakablePartComponentData))]
	public class BreakablePartComponent : DestructionPartComponent, IEntityData<FrostySdk.Ebx.BreakablePartComponentData>
	{
		public new FrostySdk.Ebx.BreakablePartComponentData Data => data as FrostySdk.Ebx.BreakablePartComponentData;
		public override string DisplayName => "BreakablePartComponent";

		public BreakablePartComponent(FrostySdk.Ebx.BreakablePartComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

