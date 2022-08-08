using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerNavigateCommandEntityData))]
	public class ProxyPlayerNavigateCommandEntity : ProxyPlayerCommandEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerNavigateCommandEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerNavigateCommandEntityData Data => data as FrostySdk.Ebx.ProxyPlayerNavigateCommandEntityData;
		public override string DisplayName => "ProxyPlayerNavigateCommand";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ProxyPlayerNavigateCommandEntity(FrostySdk.Ebx.ProxyPlayerNavigateCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

