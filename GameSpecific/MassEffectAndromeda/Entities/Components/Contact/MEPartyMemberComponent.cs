using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEPartyMemberComponentData))]
	public class MEPartyMemberComponent : PartyMemberComponent, IEntityData<FrostySdk.Ebx.MEPartyMemberComponentData>
	{
		public new FrostySdk.Ebx.MEPartyMemberComponentData Data => data as FrostySdk.Ebx.MEPartyMemberComponentData;
		public override string DisplayName => "MEPartyMemberComponent";

		public MEPartyMemberComponent(FrostySdk.Ebx.MEPartyMemberComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

