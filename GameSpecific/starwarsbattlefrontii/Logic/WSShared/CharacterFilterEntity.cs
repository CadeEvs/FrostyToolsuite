using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterFilterEntityData))]
	public class CharacterFilterEntity : FilterEntityBase, IEntityData<FrostySdk.Ebx.CharacterFilterEntityData>
	{
		public new FrostySdk.Ebx.CharacterFilterEntityData Data => data as FrostySdk.Ebx.CharacterFilterEntityData;
		public override string DisplayName => "CharacterFilter";

		public CharacterFilterEntity(FrostySdk.Ebx.CharacterFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

