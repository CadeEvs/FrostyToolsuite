using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ForceRefreshQuestDataEntityData))]
	public class ForceRefreshQuestDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ForceRefreshQuestDataEntityData>
	{
		public new FrostySdk.Ebx.ForceRefreshQuestDataEntityData Data => data as FrostySdk.Ebx.ForceRefreshQuestDataEntityData;
		public override string DisplayName => "ForceRefreshQuestData";

		public ForceRefreshQuestDataEntity(FrostySdk.Ebx.ForceRefreshQuestDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

