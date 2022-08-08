using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomizationPersistenceEntityData))]
	public class CustomizationPersistenceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CustomizationPersistenceEntityData>
	{
		public new FrostySdk.Ebx.CustomizationPersistenceEntityData Data => data as FrostySdk.Ebx.CustomizationPersistenceEntityData;
		public override string DisplayName => "CustomizationPersistence";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CustomizationPersistenceEntity(FrostySdk.Ebx.CustomizationPersistenceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

