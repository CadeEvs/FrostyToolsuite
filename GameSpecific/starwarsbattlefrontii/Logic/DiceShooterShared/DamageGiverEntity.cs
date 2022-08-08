using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageGiverEntityData))]
	public class DamageGiverEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DamageGiverEntityData>
	{
		public new FrostySdk.Ebx.DamageGiverEntityData Data => data as FrostySdk.Ebx.DamageGiverEntityData;
		public override string DisplayName => "DamageGiver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DamageGiverEntity(FrostySdk.Ebx.DamageGiverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

