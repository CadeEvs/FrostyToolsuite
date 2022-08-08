using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SenseTerrainSphereData))]
	public class SenseTerrainSphere : Sphere, IEntityData<FrostySdk.Ebx.SenseTerrainSphereData>
	{
		public new FrostySdk.Ebx.SenseTerrainSphereData Data => data as FrostySdk.Ebx.SenseTerrainSphereData;
		public override string DisplayName => "SenseTerrainSphere";

		public SenseTerrainSphere(FrostySdk.Ebx.SenseTerrainSphereData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

