using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FogComponentData))]
	public class FogComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.FogComponentData>
	{
		public new FrostySdk.Ebx.FogComponentData Data => data as FrostySdk.Ebx.FogComponentData;
		public override string DisplayName => "FogComponent";

		public FogComponent(FrostySdk.Ebx.FogComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

