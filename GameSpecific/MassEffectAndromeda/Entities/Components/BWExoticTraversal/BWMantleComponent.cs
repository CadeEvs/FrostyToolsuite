using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWMantleComponentData))]
	public class BWMantleComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWMantleComponentData>
	{
		public new FrostySdk.Ebx.BWMantleComponentData Data => data as FrostySdk.Ebx.BWMantleComponentData;
		public override string DisplayName => "BWMantleComponent";

		public BWMantleComponent(FrostySdk.Ebx.BWMantleComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

