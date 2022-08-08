using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SphereData))]
	public class Sphere : BaseShape, IEntityData<FrostySdk.Ebx.SphereData>
	{
		public new FrostySdk.Ebx.SphereData Data => data as FrostySdk.Ebx.SphereData;
		public override string DisplayName => "Sphere";

		public Sphere(FrostySdk.Ebx.SphereData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

