using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerAttackCommandEntityData))]
	public class ProxyPlayerAttackCommandEntity : ProxyPlayerCommandEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerAttackCommandEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerAttackCommandEntityData Data => data as FrostySdk.Ebx.ProxyPlayerAttackCommandEntityData;
		public override string DisplayName => "ProxyPlayerAttackCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProxyPlayerAttackCommandEntity(FrostySdk.Ebx.ProxyPlayerAttackCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

