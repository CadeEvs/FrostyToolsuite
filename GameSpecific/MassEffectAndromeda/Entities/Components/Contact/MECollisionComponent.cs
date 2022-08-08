using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECollisionComponentData))]
	public class MECollisionComponent : GameComponent, IEntityData<FrostySdk.Ebx.MECollisionComponentData>
	{
		public new FrostySdk.Ebx.MECollisionComponentData Data => data as FrostySdk.Ebx.MECollisionComponentData;
		public override string DisplayName => "MECollisionComponent";

		public MECollisionComponent(FrostySdk.Ebx.MECollisionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

