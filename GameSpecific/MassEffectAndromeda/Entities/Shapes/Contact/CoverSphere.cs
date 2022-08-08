using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverSphereData))]
	public class CoverSphere : Sphere, IEntityData<FrostySdk.Ebx.CoverSphereData>
	{
		public new FrostySdk.Ebx.CoverSphereData Data => data as FrostySdk.Ebx.CoverSphereData;
		public override string DisplayName => "CoverSphere";

		public CoverSphere(FrostySdk.Ebx.CoverSphereData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

