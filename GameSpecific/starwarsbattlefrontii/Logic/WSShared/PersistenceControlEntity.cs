using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistenceControlEntityData))]
	public class PersistenceControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PersistenceControlEntityData>
	{
		public new FrostySdk.Ebx.PersistenceControlEntityData Data => data as FrostySdk.Ebx.PersistenceControlEntityData;
		public override string DisplayName => "PersistenceControl";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PersistenceControlEntity(FrostySdk.Ebx.PersistenceControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

