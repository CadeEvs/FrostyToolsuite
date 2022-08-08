using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkyCelestialRotationComponentData))]
	public class SkyCelestialRotationComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.SkyCelestialRotationComponentData>
	{
		public new FrostySdk.Ebx.SkyCelestialRotationComponentData Data => data as FrostySdk.Ebx.SkyCelestialRotationComponentData;
		public override string DisplayName => "SkyCelestialRotationComponent";

		public SkyCelestialRotationComponent(FrostySdk.Ebx.SkyCelestialRotationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

