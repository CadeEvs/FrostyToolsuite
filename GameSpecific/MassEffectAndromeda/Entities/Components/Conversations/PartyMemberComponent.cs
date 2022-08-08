using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberComponentData))]
	public class PartyMemberComponent : GameComponent, IEntityData<FrostySdk.Ebx.PartyMemberComponentData>
	{
		public new FrostySdk.Ebx.PartyMemberComponentData Data => data as FrostySdk.Ebx.PartyMemberComponentData;
		public override string DisplayName => "PartyMemberComponent";

		public PartyMemberComponent(FrostySdk.Ebx.PartyMemberComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

