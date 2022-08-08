using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitLookupEntityData))]
	public class CharacterKitLookupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitLookupEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitLookupEntityData Data => data as FrostySdk.Ebx.CharacterKitLookupEntityData;
		public override string DisplayName => "CharacterKitLookup";

		public CharacterKitLookupEntity(FrostySdk.Ebx.CharacterKitLookupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

