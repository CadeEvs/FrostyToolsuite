using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OBBData))]
	public class OBB : BaseShape, IEntityData<FrostySdk.Ebx.OBBData>
	{
		public new FrostySdk.Ebx.OBBData Data => data as FrostySdk.Ebx.OBBData;
		public override string DisplayName => "OBB";

		public OBB(FrostySdk.Ebx.OBBData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

