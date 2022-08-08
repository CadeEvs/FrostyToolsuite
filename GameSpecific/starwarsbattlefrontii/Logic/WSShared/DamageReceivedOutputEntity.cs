using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageReceivedOutputEntityData))]
	public class DamageReceivedOutputEntity : DamageOutputEntity, IEntityData<FrostySdk.Ebx.DamageReceivedOutputEntityData>
	{
		public new FrostySdk.Ebx.DamageReceivedOutputEntityData Data => data as FrostySdk.Ebx.DamageReceivedOutputEntityData;
		public override string DisplayName => "DamageReceivedOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DamageReceivedOutputEntity(FrostySdk.Ebx.DamageReceivedOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

