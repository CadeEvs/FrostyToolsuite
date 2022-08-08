using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AITemplateFilterEntityData))]
	public class AITemplateFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AITemplateFilterEntityData>
	{
		public new FrostySdk.Ebx.AITemplateFilterEntityData Data => data as FrostySdk.Ebx.AITemplateFilterEntityData;
		public override string DisplayName => "AITemplateFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AITemplateFilterEntity(FrostySdk.Ebx.AITemplateFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

