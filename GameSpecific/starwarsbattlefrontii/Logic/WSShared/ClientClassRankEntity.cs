using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientClassRankEntityData))]
	public class ClientClassRankEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientClassRankEntityData>
	{
		public new FrostySdk.Ebx.ClientClassRankEntityData Data => data as FrostySdk.Ebx.ClientClassRankEntityData;
		public override string DisplayName => "ClientClassRank";

		public ClientClassRankEntity(FrostySdk.Ebx.ClientClassRankEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

