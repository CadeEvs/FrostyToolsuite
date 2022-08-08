using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProtectBaseManeuverEntityData))]
	public class ProtectBaseManeuverEntity : DogFightManeuverEntityBase, IEntityData<FrostySdk.Ebx.ProtectBaseManeuverEntityData>
	{
		public new FrostySdk.Ebx.ProtectBaseManeuverEntityData Data => data as FrostySdk.Ebx.ProtectBaseManeuverEntityData;
		public override string DisplayName => "ProtectBaseManeuver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProtectBaseManeuverEntity(FrostySdk.Ebx.ProtectBaseManeuverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

