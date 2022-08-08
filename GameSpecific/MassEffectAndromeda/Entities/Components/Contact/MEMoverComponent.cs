using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEMoverComponentData))]
	public class MEMoverComponent : MoverComponent, IEntityData<FrostySdk.Ebx.MEMoverComponentData>
	{
		public new FrostySdk.Ebx.MEMoverComponentData Data => data as FrostySdk.Ebx.MEMoverComponentData;
		public override string DisplayName => "MEMoverComponent";

		public MEMoverComponent(FrostySdk.Ebx.MEMoverComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

