using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BasicAttackManeuverEntityData))]
	public class BasicAttackManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.BasicAttackManeuverEntityData>
	{
		public new FrostySdk.Ebx.BasicAttackManeuverEntityData Data => data as FrostySdk.Ebx.BasicAttackManeuverEntityData;
		public override string DisplayName => "BasicAttackManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BasicAttackManeuverEntity(FrostySdk.Ebx.BasicAttackManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

