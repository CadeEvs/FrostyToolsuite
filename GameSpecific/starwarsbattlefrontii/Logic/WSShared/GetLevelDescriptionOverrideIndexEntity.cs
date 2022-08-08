using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetLevelDescriptionOverrideIndexEntityData))]
	public class GetLevelDescriptionOverrideIndexEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetLevelDescriptionOverrideIndexEntityData>
	{
		public new FrostySdk.Ebx.GetLevelDescriptionOverrideIndexEntityData Data => data as FrostySdk.Ebx.GetLevelDescriptionOverrideIndexEntityData;
		public override string DisplayName => "GetLevelDescriptionOverrideIndex";

		public GetLevelDescriptionOverrideIndexEntity(FrostySdk.Ebx.GetLevelDescriptionOverrideIndexEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

