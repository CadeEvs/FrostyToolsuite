using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectableActionEntityData))]
	public class SelectableActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SelectableActionEntityData>
	{
		public new FrostySdk.Ebx.SelectableActionEntityData Data => data as FrostySdk.Ebx.SelectableActionEntityData;
		public override string DisplayName => "SelectableAction";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SelectableActionEntity(FrostySdk.Ebx.SelectableActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

