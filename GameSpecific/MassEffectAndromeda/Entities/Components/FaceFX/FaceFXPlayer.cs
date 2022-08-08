using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FaceFXPlayerData))]
	public class FaceFXPlayer : GameComponent, IEntityData<FrostySdk.Ebx.FaceFXPlayerData>
	{
		public new FrostySdk.Ebx.FaceFXPlayerData Data => data as FrostySdk.Ebx.FaceFXPlayerData;
		public override string DisplayName => "FaceFXPlayer";

		public FaceFXPlayer(FrostySdk.Ebx.FaceFXPlayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

