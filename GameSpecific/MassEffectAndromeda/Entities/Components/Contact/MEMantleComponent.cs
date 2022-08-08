using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEMantleComponentData))]
	public class MEMantleComponent : BWMantleComponent, IEntityData<FrostySdk.Ebx.MEMantleComponentData>
	{
		public new FrostySdk.Ebx.MEMantleComponentData Data => data as FrostySdk.Ebx.MEMantleComponentData;
		public override string DisplayName => "MEMantleComponent";

		public MEMantleComponent(FrostySdk.Ebx.MEMantleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

