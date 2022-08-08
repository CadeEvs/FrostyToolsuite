using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageOutputEntityData))]
	public class DamageOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DamageOutputEntityData>
	{
		public new FrostySdk.Ebx.DamageOutputEntityData Data => data as FrostySdk.Ebx.DamageOutputEntityData;
		public override string DisplayName => "DamageOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DamageOutputEntity(FrostySdk.Ebx.DamageOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

