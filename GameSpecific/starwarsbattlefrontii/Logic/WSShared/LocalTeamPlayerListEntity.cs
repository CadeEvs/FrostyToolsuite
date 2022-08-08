using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalTeamPlayerListEntityData))]
	public class LocalTeamPlayerListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalTeamPlayerListEntityData>
	{
		public new FrostySdk.Ebx.LocalTeamPlayerListEntityData Data => data as FrostySdk.Ebx.LocalTeamPlayerListEntityData;
		public override string DisplayName => "LocalTeamPlayerList";

		public LocalTeamPlayerListEntity(FrostySdk.Ebx.LocalTeamPlayerListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

