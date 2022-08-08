using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerAttackNearestEnemyCommandEntityData))]
	public class ProxyPlayerAttackNearestEnemyCommandEntity : ProxyPlayerCommandEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerAttackNearestEnemyCommandEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerAttackNearestEnemyCommandEntityData Data => data as FrostySdk.Ebx.ProxyPlayerAttackNearestEnemyCommandEntityData;
		public override string DisplayName => "ProxyPlayerAttackNearestEnemyCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProxyPlayerAttackNearestEnemyCommandEntity(FrostySdk.Ebx.ProxyPlayerAttackNearestEnemyCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

