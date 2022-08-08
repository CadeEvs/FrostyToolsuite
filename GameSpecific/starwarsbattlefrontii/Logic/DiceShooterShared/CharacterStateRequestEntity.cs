using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterStateRequestEntityData))]
	public class CharacterStateRequestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterStateRequestEntityData>
	{
		public new FrostySdk.Ebx.CharacterStateRequestEntityData Data => data as FrostySdk.Ebx.CharacterStateRequestEntityData;
		public override string DisplayName => "CharacterStateRequest";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CharacterStateRequestEntity(FrostySdk.Ebx.CharacterStateRequestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

