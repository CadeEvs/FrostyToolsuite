using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESensibleComponentData))]
	public class MESensibleComponent : SensibleComponent, IEntityData<FrostySdk.Ebx.MESensibleComponentData>
	{
		public new FrostySdk.Ebx.MESensibleComponentData Data => data as FrostySdk.Ebx.MESensibleComponentData;
		public override string DisplayName => "MESensibleComponent";

		public MESensibleComponent(FrostySdk.Ebx.MESensibleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

