using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWCSMTargetComponentData))]
	public class BWCSMTargetComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWCSMTargetComponentData>
	{
		public new FrostySdk.Ebx.BWCSMTargetComponentData Data => data as FrostySdk.Ebx.BWCSMTargetComponentData;
		public override string DisplayName => "BWCSMTargetComponent";

		public BWCSMTargetComponent(FrostySdk.Ebx.BWCSMTargetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

