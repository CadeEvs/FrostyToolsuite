using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MantleSphereData))]
	public class MantleSphere : Sphere, IEntityData<FrostySdk.Ebx.MantleSphereData>
	{
		public new FrostySdk.Ebx.MantleSphereData Data => data as FrostySdk.Ebx.MantleSphereData;
		public override string DisplayName => "MantleSphere";

		public MantleSphere(FrostySdk.Ebx.MantleSphereData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

