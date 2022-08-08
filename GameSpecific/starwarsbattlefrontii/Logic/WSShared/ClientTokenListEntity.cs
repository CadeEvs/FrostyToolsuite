using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientTokenListEntityData))]
	public class ClientTokenListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientTokenListEntityData>
	{
		public new FrostySdk.Ebx.ClientTokenListEntityData Data => data as FrostySdk.Ebx.ClientTokenListEntityData;
		public override string DisplayName => "ClientTokenList";

		public ClientTokenListEntity(FrostySdk.Ebx.ClientTokenListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

