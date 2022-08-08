using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QuestFilterEntityData))]
	public class QuestFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QuestFilterEntityData>
	{
		public new FrostySdk.Ebx.QuestFilterEntityData Data => data as FrostySdk.Ebx.QuestFilterEntityData;
		public override string DisplayName => "QuestFilter";

		public QuestFilterEntity(FrostySdk.Ebx.QuestFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

