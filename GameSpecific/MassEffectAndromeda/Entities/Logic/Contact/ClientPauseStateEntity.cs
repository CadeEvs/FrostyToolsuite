using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPauseStateEntityData))]
	public class ClientPauseStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPauseStateEntityData>
	{
		public new FrostySdk.Ebx.ClientPauseStateEntityData Data => data as FrostySdk.Ebx.ClientPauseStateEntityData;
		public override string DisplayName => "ClientPauseState";

		public ClientPauseStateEntity(FrostySdk.Ebx.ClientPauseStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

