using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerMoveToCommandEntityData))]
	public class ProxyPlayerMoveToCommandEntity : ProxyPlayerCommandEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerMoveToCommandEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerMoveToCommandEntityData Data => data as FrostySdk.Ebx.ProxyPlayerMoveToCommandEntityData;
		public override string DisplayName => "ProxyPlayerMoveToCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProxyPlayerMoveToCommandEntity(FrostySdk.Ebx.ProxyPlayerMoveToCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

