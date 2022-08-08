using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWCSMComponentData))]
	public class BWCSMComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWCSMComponentData>
	{
		public new FrostySdk.Ebx.BWCSMComponentData Data => data as FrostySdk.Ebx.BWCSMComponentData;
		public override string DisplayName => "BWCSMComponent";

		public BWCSMComponent(FrostySdk.Ebx.BWCSMComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

