using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameSplineData))]
	public class GameSpline : CustomSpline, IEntityData<FrostySdk.Ebx.GameSplineData>
	{
		public new FrostySdk.Ebx.GameSplineData Data => data as FrostySdk.Ebx.GameSplineData;
		public override string DisplayName => "GameSpline";

		public GameSpline(FrostySdk.Ebx.GameSplineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

