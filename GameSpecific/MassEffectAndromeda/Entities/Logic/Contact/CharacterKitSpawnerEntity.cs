using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitSpawnerEntityData))]
	public class CharacterKitSpawnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitSpawnerEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitSpawnerEntityData Data => data as FrostySdk.Ebx.CharacterKitSpawnerEntityData;
		public override string DisplayName => "CharacterKitSpawner";

		public CharacterKitSpawnerEntity(FrostySdk.Ebx.CharacterKitSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

