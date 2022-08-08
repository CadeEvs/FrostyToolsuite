using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientTokenLookupEntityData))]
	public class ClientTokenLookupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientTokenLookupEntityData>
	{
		public new FrostySdk.Ebx.ClientTokenLookupEntityData Data => data as FrostySdk.Ebx.ClientTokenLookupEntityData;
		public override string DisplayName => "ClientTokenLookup";

		public ClientTokenLookupEntity(FrostySdk.Ebx.ClientTokenLookupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

