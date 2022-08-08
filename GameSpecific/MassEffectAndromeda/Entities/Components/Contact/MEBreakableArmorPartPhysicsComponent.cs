using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEBreakableArmorPartPhysicsComponentData))]
	public class MEBreakableArmorPartPhysicsComponent : StaticModelPhysicsComponent, IEntityData<FrostySdk.Ebx.MEBreakableArmorPartPhysicsComponentData>
	{
		public new FrostySdk.Ebx.MEBreakableArmorPartPhysicsComponentData Data => data as FrostySdk.Ebx.MEBreakableArmorPartPhysicsComponentData;
		public override string DisplayName => "MEBreakableArmorPartPhysicsComponent";

		public MEBreakableArmorPartPhysicsComponent(FrostySdk.Ebx.MEBreakableArmorPartPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

