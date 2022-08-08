using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEDynamicExclusionComponentData))]
	public class MEDynamicExclusionComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEDynamicExclusionComponentData>
	{
		public new FrostySdk.Ebx.MEDynamicExclusionComponentData Data => data as FrostySdk.Ebx.MEDynamicExclusionComponentData;
		public override string DisplayName => "MEDynamicExclusionComponent";

		public MEDynamicExclusionComponent(FrostySdk.Ebx.MEDynamicExclusionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

