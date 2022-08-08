using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEExertionComponentData))]
	public class MEExertionComponent : ExertionComponent, IEntityData<FrostySdk.Ebx.MEExertionComponentData>
	{
		public new FrostySdk.Ebx.MEExertionComponentData Data => data as FrostySdk.Ebx.MEExertionComponentData;
		public override string DisplayName => "MEExertionComponent";

		public MEExertionComponent(FrostySdk.Ebx.MEExertionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

