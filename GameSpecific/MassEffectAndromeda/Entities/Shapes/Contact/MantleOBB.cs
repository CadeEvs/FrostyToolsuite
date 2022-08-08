using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MantleOBBData))]
	public class MantleOBB : OBB, IEntityData<FrostySdk.Ebx.MantleOBBData>
	{
		public new FrostySdk.Ebx.MantleOBBData Data => data as FrostySdk.Ebx.MantleOBBData;
		public override string DisplayName => "MantleOBB";

		public MantleOBB(FrostySdk.Ebx.MantleOBBData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

