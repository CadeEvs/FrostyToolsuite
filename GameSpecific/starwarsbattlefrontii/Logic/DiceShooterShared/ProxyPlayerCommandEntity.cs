using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerCommandEntityData))]
	public class ProxyPlayerCommandEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerCommandEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerCommandEntityData Data => data as FrostySdk.Ebx.ProxyPlayerCommandEntityData;
		public override string DisplayName => "ProxyPlayerCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProxyPlayerCommandEntity(FrostySdk.Ebx.ProxyPlayerCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

