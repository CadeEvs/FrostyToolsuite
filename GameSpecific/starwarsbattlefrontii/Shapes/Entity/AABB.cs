using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AABBData))]
	public class AABB : BaseShape, IEntityData<FrostySdk.Ebx.AABBData>
	{
		public new FrostySdk.Ebx.AABBData Data => data as FrostySdk.Ebx.AABBData;
		public override string DisplayName => "AABB";

		public AABB(FrostySdk.Ebx.AABBData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

