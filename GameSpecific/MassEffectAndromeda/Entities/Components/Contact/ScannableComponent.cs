using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScannableComponentData))]
	public class ScannableComponent : GameComponent, IEntityData<FrostySdk.Ebx.ScannableComponentData>
	{
		public new FrostySdk.Ebx.ScannableComponentData Data => data as FrostySdk.Ebx.ScannableComponentData;
		public override string DisplayName => "ScannableComponent";

		public ScannableComponent(FrostySdk.Ebx.ScannableComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

