using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadComponentData))]
	public class SquadComponent : GameComponent, IEntityData<FrostySdk.Ebx.SquadComponentData>
	{
		public new FrostySdk.Ebx.SquadComponentData Data => data as FrostySdk.Ebx.SquadComponentData;
		public override string DisplayName => "SquadComponent";

		public SquadComponent(FrostySdk.Ebx.SquadComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

