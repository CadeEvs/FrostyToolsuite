using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BuddySpawnManagerEntityData))]
	public class BuddySpawnManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BuddySpawnManagerEntityData>
	{
		public new FrostySdk.Ebx.BuddySpawnManagerEntityData Data => data as FrostySdk.Ebx.BuddySpawnManagerEntityData;
		public override string DisplayName => "BuddySpawnManager";

		public BuddySpawnManagerEntity(FrostySdk.Ebx.BuddySpawnManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

