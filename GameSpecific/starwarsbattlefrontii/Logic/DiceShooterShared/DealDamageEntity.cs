using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DealDamageEntityData))]
	public class DealDamageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DealDamageEntityData>
	{
		public new FrostySdk.Ebx.DealDamageEntityData Data => data as FrostySdk.Ebx.DealDamageEntityData;
		public override string DisplayName => "DealDamage";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DealDamageEntity(FrostySdk.Ebx.DealDamageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

