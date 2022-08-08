using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MakoEngineComponentData))]
	public class MakoEngineComponent : MEEngineComponent, IEntityData<FrostySdk.Ebx.MakoEngineComponentData>
	{
		public new FrostySdk.Ebx.MakoEngineComponentData Data => data as FrostySdk.Ebx.MakoEngineComponentData;
		public override string DisplayName => "MakoEngineComponent";

		public MakoEngineComponent(FrostySdk.Ebx.MakoEngineComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

