using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterManagerDataProviderData))]
	public class CharacterManagerDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterManagerDataProviderData>
	{
		public new FrostySdk.Ebx.CharacterManagerDataProviderData Data => data as FrostySdk.Ebx.CharacterManagerDataProviderData;
		public override string DisplayName => "CharacterManagerDataProvider";

		public CharacterManagerDataProvider(FrostySdk.Ebx.CharacterManagerDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

