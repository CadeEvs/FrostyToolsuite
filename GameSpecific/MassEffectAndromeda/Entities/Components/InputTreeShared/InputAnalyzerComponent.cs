using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputAnalyzerComponentData))]
	public class InputAnalyzerComponent : InputTreeBaseComponent, IEntityData<FrostySdk.Ebx.InputAnalyzerComponentData>
	{
		public new FrostySdk.Ebx.InputAnalyzerComponentData Data => data as FrostySdk.Ebx.InputAnalyzerComponentData;
		public override string DisplayName => "InputAnalyzerComponent";

		public InputAnalyzerComponent(FrostySdk.Ebx.InputAnalyzerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

