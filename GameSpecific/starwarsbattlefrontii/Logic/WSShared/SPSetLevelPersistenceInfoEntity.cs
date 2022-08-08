using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPSetLevelPersistenceInfoEntityData))]
	public class SPSetLevelPersistenceInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPSetLevelPersistenceInfoEntityData>
	{
		public new FrostySdk.Ebx.SPSetLevelPersistenceInfoEntityData Data => data as FrostySdk.Ebx.SPSetLevelPersistenceInfoEntityData;
		public override string DisplayName => "SPSetLevelPersistenceInfo";

		public SPSetLevelPersistenceInfoEntity(FrostySdk.Ebx.SPSetLevelPersistenceInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

