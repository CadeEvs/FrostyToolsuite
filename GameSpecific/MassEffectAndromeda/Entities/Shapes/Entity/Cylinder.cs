using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CylinderData))]
	public class Cylinder : OBB, IEntityData<FrostySdk.Ebx.CylinderData>
	{
		public new FrostySdk.Ebx.CylinderData Data => data as FrostySdk.Ebx.CylinderData;
		public override string DisplayName => "Cylinder";

		public Cylinder(FrostySdk.Ebx.CylinderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

