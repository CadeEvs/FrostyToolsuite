using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterClassFilterEntityData))]
	public class CharacterClassFilterEntity : FilterEntityBase, IEntityData<FrostySdk.Ebx.CharacterClassFilterEntityData>
	{
		public new FrostySdk.Ebx.CharacterClassFilterEntityData Data => data as FrostySdk.Ebx.CharacterClassFilterEntityData;
		public override string DisplayName => "CharacterClassFilter";

		public CharacterClassFilterEntity(FrostySdk.Ebx.CharacterClassFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

