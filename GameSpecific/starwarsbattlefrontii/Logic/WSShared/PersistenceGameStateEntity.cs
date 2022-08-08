using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistenceGameStateEntityData))]
	public class PersistenceGameStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PersistenceGameStateEntityData>
	{
		public new FrostySdk.Ebx.PersistenceGameStateEntityData Data => data as FrostySdk.Ebx.PersistenceGameStateEntityData;
		public override string DisplayName => "PersistenceGameState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PersistenceGameStateEntity(FrostySdk.Ebx.PersistenceGameStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

