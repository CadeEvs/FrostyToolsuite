using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DofComponentData))]
	public class DofComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.DofComponentData>
	{
		public new FrostySdk.Ebx.DofComponentData Data => data as FrostySdk.Ebx.DofComponentData;
		public override string DisplayName => "DofComponent";

		public DofComponent(FrostySdk.Ebx.DofComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

