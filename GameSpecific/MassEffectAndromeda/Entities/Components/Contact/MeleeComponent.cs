using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeleeComponentData))]
	public class MeleeComponent : GameComponent, IEntityData<FrostySdk.Ebx.MeleeComponentData>
	{
		public new FrostySdk.Ebx.MeleeComponentData Data => data as FrostySdk.Ebx.MeleeComponentData;
		public override string DisplayName => "MeleeComponent";

		public MeleeComponent(FrostySdk.Ebx.MeleeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

