using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundVisualizationElementData))]
	public class SoundVisualizationElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.SoundVisualizationElementData>
	{
		public new FrostySdk.Ebx.SoundVisualizationElementData Data => data as FrostySdk.Ebx.SoundVisualizationElementData;
		public override string DisplayName => "SoundVisualizationElement";

		public SoundVisualizationElement(FrostySdk.Ebx.SoundVisualizationElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

