using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BuddySpawnEntityData))]
	public class BuddySpawnEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BuddySpawnEntityData>
	{
		public new FrostySdk.Ebx.BuddySpawnEntityData Data => data as FrostySdk.Ebx.BuddySpawnEntityData;
		public override string DisplayName => "BuddySpawn";

		public BuddySpawnEntity(FrostySdk.Ebx.BuddySpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

