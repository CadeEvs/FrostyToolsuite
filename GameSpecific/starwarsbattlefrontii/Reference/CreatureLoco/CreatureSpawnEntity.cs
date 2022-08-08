using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureSpawnEntityData))]
	public class CreatureSpawnEntity : BlueprintSpawnReferenceObject, IEntityData<FrostySdk.Ebx.CreatureSpawnEntityData>
	{
		public new FrostySdk.Ebx.CreatureSpawnEntityData Data => data as FrostySdk.Ebx.CreatureSpawnEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureSpawnEntity(FrostySdk.Ebx.CreatureSpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

