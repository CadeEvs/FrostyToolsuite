using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2TargetComponentData))]
	public class PA2TargetComponent : GameComponent, IEntityData<FrostySdk.Ebx.PA2TargetComponentData>
	{
		public new FrostySdk.Ebx.PA2TargetComponentData Data => data as FrostySdk.Ebx.PA2TargetComponentData;
		public override string DisplayName => "PA2TargetComponent";

		public PA2TargetComponent(FrostySdk.Ebx.PA2TargetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

