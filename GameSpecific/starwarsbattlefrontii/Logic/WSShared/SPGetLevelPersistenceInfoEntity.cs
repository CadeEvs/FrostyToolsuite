using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPGetLevelPersistenceInfoEntityData))]
	public class SPGetLevelPersistenceInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPGetLevelPersistenceInfoEntityData>
	{
		public new FrostySdk.Ebx.SPGetLevelPersistenceInfoEntityData Data => data as FrostySdk.Ebx.SPGetLevelPersistenceInfoEntityData;
		public override string DisplayName => "SPGetLevelPersistenceInfo";

		public SPGetLevelPersistenceInfoEntity(FrostySdk.Ebx.SPGetLevelPersistenceInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

