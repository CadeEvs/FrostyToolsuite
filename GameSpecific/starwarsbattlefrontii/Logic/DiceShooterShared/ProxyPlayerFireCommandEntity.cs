using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerFireCommandEntityData))]
	public class ProxyPlayerFireCommandEntity : ProxyPlayerCommandEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerFireCommandEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerFireCommandEntityData Data => data as FrostySdk.Ebx.ProxyPlayerFireCommandEntityData;
		public override string DisplayName => "ProxyPlayerFireCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProxyPlayerFireCommandEntity(FrostySdk.Ebx.ProxyPlayerFireCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

