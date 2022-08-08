using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWTargetComponentData))]
	public class BWTargetComponent : BWCSMTargetComponent, IEntityData<FrostySdk.Ebx.BWTargetComponentData>
	{
		public new FrostySdk.Ebx.BWTargetComponentData Data => data as FrostySdk.Ebx.BWTargetComponentData;
		public override string DisplayName => "BWTargetComponent";

		public BWTargetComponent(FrostySdk.Ebx.BWTargetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

