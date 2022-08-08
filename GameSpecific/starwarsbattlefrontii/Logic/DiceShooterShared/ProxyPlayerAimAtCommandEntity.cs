using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerAimAtCommandEntityData))]
	public class ProxyPlayerAimAtCommandEntity : ProxyPlayerCommandEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerAimAtCommandEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerAimAtCommandEntityData Data => data as FrostySdk.Ebx.ProxyPlayerAimAtCommandEntityData;
		public override string DisplayName => "ProxyPlayerAimAtCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProxyPlayerAimAtCommandEntity(FrostySdk.Ebx.ProxyPlayerAimAtCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

