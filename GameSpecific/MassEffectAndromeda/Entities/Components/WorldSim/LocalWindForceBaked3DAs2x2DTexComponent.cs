using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexComponentData))]
	public class LocalWindForceBaked3DAs2x2DTexComponent : LocalWindForceComponentBase, IEntityData<FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexComponentData>
	{
		public new FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexComponentData Data => data as FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexComponentData;
		public override string DisplayName => "LocalWindForceBaked3DAs2x2DTexComponent";

		public LocalWindForceBaked3DAs2x2DTexComponent(FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

