using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CurrentLevelEntityData))]
	public class CurrentLevelEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CurrentLevelEntityData>
	{
		public new FrostySdk.Ebx.CurrentLevelEntityData Data => data as FrostySdk.Ebx.CurrentLevelEntityData;
		public override string DisplayName => "CurrentLevel";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CurrentLevelEntity(FrostySdk.Ebx.CurrentLevelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

