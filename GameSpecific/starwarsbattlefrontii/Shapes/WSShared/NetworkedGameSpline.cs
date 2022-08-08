using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NetworkedGameSplineData))]
	public class NetworkedGameSpline : GameSpline, IEntityData<FrostySdk.Ebx.NetworkedGameSplineData>
	{
		public new FrostySdk.Ebx.NetworkedGameSplineData Data => data as FrostySdk.Ebx.NetworkedGameSplineData;
		public override string DisplayName => "NetworkedGameSpline";

		public NetworkedGameSpline(FrostySdk.Ebx.NetworkedGameSplineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

