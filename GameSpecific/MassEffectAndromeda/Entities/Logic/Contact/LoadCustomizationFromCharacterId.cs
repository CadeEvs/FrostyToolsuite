using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadCustomizationFromCharacterIdData))]
	public class LoadCustomizationFromCharacterId : LogicEntity, IEntityData<FrostySdk.Ebx.LoadCustomizationFromCharacterIdData>
	{
		public new FrostySdk.Ebx.LoadCustomizationFromCharacterIdData Data => data as FrostySdk.Ebx.LoadCustomizationFromCharacterIdData;
		public override string DisplayName => "LoadCustomizationFromCharacterId";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LoadCustomizationFromCharacterId(FrostySdk.Ebx.LoadCustomizationFromCharacterIdData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

