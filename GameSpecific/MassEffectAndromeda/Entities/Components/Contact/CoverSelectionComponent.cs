using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverSelectionComponentData))]
	public class CoverSelectionComponent : GameComponent, IEntityData<FrostySdk.Ebx.CoverSelectionComponentData>
	{
		public new FrostySdk.Ebx.CoverSelectionComponentData Data => data as FrostySdk.Ebx.CoverSelectionComponentData;
		public override string DisplayName => "CoverSelectionComponent";

		public CoverSelectionComponent(FrostySdk.Ebx.CoverSelectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

