using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterSpawnEntityData))]
	public class CharacterSpawnEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterSpawnEntityData>
	{
		public new FrostySdk.Ebx.CharacterSpawnEntityData Data => data as FrostySdk.Ebx.CharacterSpawnEntityData;
		public override string DisplayName => "CharacterSpawn";

		public CharacterSpawnEntity(FrostySdk.Ebx.CharacterSpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

