using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomizeCharacterEntityData))]
	public class CustomizeCharacterEntity : CustomizeBaseEntity, IEntityData<FrostySdk.Ebx.CustomizeCharacterEntityData>
	{
		public new FrostySdk.Ebx.CustomizeCharacterEntityData Data => data as FrostySdk.Ebx.CustomizeCharacterEntityData;
		public override string DisplayName => "CustomizeCharacter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CustomizeCharacterEntity(FrostySdk.Ebx.CustomizeCharacterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

