using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceSphereComponentData))]
	public class LocalWindForceSphereComponent : LocalWindForceComponentBase, IEntityData<FrostySdk.Ebx.LocalWindForceSphereComponentData>
	{
		public new FrostySdk.Ebx.LocalWindForceSphereComponentData Data => data as FrostySdk.Ebx.LocalWindForceSphereComponentData;
		public override string DisplayName => "LocalWindForceSphereComponent";

		public LocalWindForceSphereComponent(FrostySdk.Ebx.LocalWindForceSphereComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

