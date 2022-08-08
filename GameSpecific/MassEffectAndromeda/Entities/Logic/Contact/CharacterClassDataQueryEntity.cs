using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterClassDataQueryEntityData))]
	public class CharacterClassDataQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterClassDataQueryEntityData>
	{
		public new FrostySdk.Ebx.CharacterClassDataQueryEntityData Data => data as FrostySdk.Ebx.CharacterClassDataQueryEntityData;
		public override string DisplayName => "CharacterClassDataQuery";

		public CharacterClassDataQueryEntity(FrostySdk.Ebx.CharacterClassDataQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

