using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RotorComponentData))]
	public class RotorComponent : GameComponent, IEntityData<FrostySdk.Ebx.RotorComponentData>
	{
		public new FrostySdk.Ebx.RotorComponentData Data => data as FrostySdk.Ebx.RotorComponentData;
		public override string DisplayName => "RotorComponent";

		public RotorComponent(FrostySdk.Ebx.RotorComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

