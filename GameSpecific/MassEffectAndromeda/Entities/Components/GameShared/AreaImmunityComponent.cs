using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AreaImmunityComponentData))]
	public class AreaImmunityComponent : GameComponent, IEntityData<FrostySdk.Ebx.AreaImmunityComponentData>
	{
		public new FrostySdk.Ebx.AreaImmunityComponentData Data => data as FrostySdk.Ebx.AreaImmunityComponentData;
		public override string DisplayName => "AreaImmunityComponent";

		public AreaImmunityComponent(FrostySdk.Ebx.AreaImmunityComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

