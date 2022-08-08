using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureCollisionGroupData))]
	public class CreatureCollisionGroup : LogicEntity, IEntityData<FrostySdk.Ebx.CreatureCollisionGroupData>
	{
		public new FrostySdk.Ebx.CreatureCollisionGroupData Data => data as FrostySdk.Ebx.CreatureCollisionGroupData;
		public override string DisplayName => "CreatureCollisionGroup";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreatureCollisionGroup(FrostySdk.Ebx.CreatureCollisionGroupData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

