using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverOBBData))]
	public class CoverOBB : OBB, IEntityData<FrostySdk.Ebx.CoverOBBData>
	{
		public new FrostySdk.Ebx.CoverOBBData Data => data as FrostySdk.Ebx.CoverOBBData;
		public override string DisplayName => "CoverOBB";

		public CoverOBB(FrostySdk.Ebx.CoverOBBData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

