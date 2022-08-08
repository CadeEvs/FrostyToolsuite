using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RibbonData))]
	public class Ribbon : VisualVectorShape, IEntityData<FrostySdk.Ebx.RibbonData>
	{
		public new FrostySdk.Ebx.RibbonData Data => data as FrostySdk.Ebx.RibbonData;
		public override string DisplayName => "Ribbon";

		public Ribbon(FrostySdk.Ebx.RibbonData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

