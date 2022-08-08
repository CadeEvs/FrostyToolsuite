using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnManagerEntityData))]
	public class SpawnManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnManagerEntityData>
	{
		public new FrostySdk.Ebx.SpawnManagerEntityData Data => data as FrostySdk.Ebx.SpawnManagerEntityData;
		public override string DisplayName => "SpawnManager";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpawnManagerEntity(FrostySdk.Ebx.SpawnManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

