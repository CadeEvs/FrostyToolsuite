using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RocketEngineComponentData))]
	public class RocketEngineComponent : MEEngineComponent, IEntityData<FrostySdk.Ebx.RocketEngineComponentData>
	{
		public new FrostySdk.Ebx.RocketEngineComponentData Data => data as FrostySdk.Ebx.RocketEngineComponentData;
		public override string DisplayName => "RocketEngineComponent";

		public RocketEngineComponent(FrostySdk.Ebx.RocketEngineComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

