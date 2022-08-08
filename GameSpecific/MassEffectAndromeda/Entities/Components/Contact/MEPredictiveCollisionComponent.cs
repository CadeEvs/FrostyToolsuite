using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEPredictiveCollisionComponentData))]
	public class MEPredictiveCollisionComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEPredictiveCollisionComponentData>
	{
		public new FrostySdk.Ebx.MEPredictiveCollisionComponentData Data => data as FrostySdk.Ebx.MEPredictiveCollisionComponentData;
		public override string DisplayName => "MEPredictiveCollisionComponent";

		public MEPredictiveCollisionComponent(FrostySdk.Ebx.MEPredictiveCollisionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

