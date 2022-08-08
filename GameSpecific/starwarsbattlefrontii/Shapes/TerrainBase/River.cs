using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RiverData))]
	public class River : Ribbon, IEntityData<FrostySdk.Ebx.RiverData>
	{
		public new FrostySdk.Ebx.RiverData Data => data as FrostySdk.Ebx.RiverData;
		public override string DisplayName => "River";

		public River(FrostySdk.Ebx.RiverData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

