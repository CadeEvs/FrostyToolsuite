using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RoadData))]
	public class Road : Ribbon, IEntityData<FrostySdk.Ebx.RoadData>
	{
		public new FrostySdk.Ebx.RoadData Data => data as FrostySdk.Ebx.RoadData;
		public override string DisplayName => "Road";

		public Road(FrostySdk.Ebx.RoadData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

