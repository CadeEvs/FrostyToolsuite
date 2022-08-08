using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClothCollisionComponentData))]
	public class ClothCollisionComponent : Component, IEntityData<FrostySdk.Ebx.ClothCollisionComponentData>
	{
		public new FrostySdk.Ebx.ClothCollisionComponentData Data => data as FrostySdk.Ebx.ClothCollisionComponentData;
		public override string DisplayName => "ClothCollisionComponent";

		public ClothCollisionComponent(FrostySdk.Ebx.ClothCollisionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

