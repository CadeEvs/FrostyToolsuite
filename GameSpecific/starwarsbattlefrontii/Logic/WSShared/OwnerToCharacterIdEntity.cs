using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OwnerToCharacterIdEntityData))]
	public class OwnerToCharacterIdEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OwnerToCharacterIdEntityData>
	{
		public new FrostySdk.Ebx.OwnerToCharacterIdEntityData Data => data as FrostySdk.Ebx.OwnerToCharacterIdEntityData;
		public override string DisplayName => "OwnerToCharacterId";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OwnerToCharacterIdEntity(FrostySdk.Ebx.OwnerToCharacterIdEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

