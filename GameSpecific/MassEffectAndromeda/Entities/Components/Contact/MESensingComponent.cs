using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESensingComponentData))]
	public class MESensingComponent : BWSensingComponent, IEntityData<FrostySdk.Ebx.MESensingComponentData>
	{
		public new FrostySdk.Ebx.MESensingComponentData Data => data as FrostySdk.Ebx.MESensingComponentData;
		public override string DisplayName => "MESensingComponent";

		public MESensingComponent(FrostySdk.Ebx.MESensingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

