using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeakPointsComponentData))]
	public class WeakPointsComponent : GameComponent, IEntityData<FrostySdk.Ebx.WeakPointsComponentData>
	{
		public new FrostySdk.Ebx.WeakPointsComponentData Data => data as FrostySdk.Ebx.WeakPointsComponentData;
		public override string DisplayName => "WeakPointsComponent";

		public WeakPointsComponent(FrostySdk.Ebx.WeakPointsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

