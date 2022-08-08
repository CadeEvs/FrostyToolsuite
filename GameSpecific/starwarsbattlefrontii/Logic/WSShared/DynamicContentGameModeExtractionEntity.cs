using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicContentGameModeExtractionEntityData))]
	public class DynamicContentGameModeExtractionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicContentGameModeExtractionEntityData>
	{
		public new FrostySdk.Ebx.DynamicContentGameModeExtractionEntityData Data => data as FrostySdk.Ebx.DynamicContentGameModeExtractionEntityData;
		public override string DisplayName => "DynamicContentGameModeExtraction";

		public DynamicContentGameModeExtractionEntity(FrostySdk.Ebx.DynamicContentGameModeExtractionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

