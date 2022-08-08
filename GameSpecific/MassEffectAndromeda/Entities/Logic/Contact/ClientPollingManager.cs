using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPollingManagerData))]
	public class ClientPollingManager : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPollingManagerData>
	{
		public new FrostySdk.Ebx.ClientPollingManagerData Data => data as FrostySdk.Ebx.ClientPollingManagerData;
		public override string DisplayName => "ClientPollingManager";

		public ClientPollingManager(FrostySdk.Ebx.ClientPollingManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

