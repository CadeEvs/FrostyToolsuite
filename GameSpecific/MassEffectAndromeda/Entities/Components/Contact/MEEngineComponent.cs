using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEEngineComponentData))]
	public class MEEngineComponent : EngineComponent, IEntityData<FrostySdk.Ebx.MEEngineComponentData>
	{
		public new FrostySdk.Ebx.MEEngineComponentData Data => data as FrostySdk.Ebx.MEEngineComponentData;
		public override string DisplayName => "MEEngineComponent";

		public MEEngineComponent(FrostySdk.Ebx.MEEngineComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

