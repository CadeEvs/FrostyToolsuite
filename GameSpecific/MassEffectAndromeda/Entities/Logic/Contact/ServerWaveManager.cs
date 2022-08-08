using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerWaveManagerData))]
	public class ServerWaveManager : LogicEntity, IEntityData<FrostySdk.Ebx.ServerWaveManagerData>
	{
		public new FrostySdk.Ebx.ServerWaveManagerData Data => data as FrostySdk.Ebx.ServerWaveManagerData;
		public override string DisplayName => "ServerWaveManager";

		public ServerWaveManager(FrostySdk.Ebx.ServerWaveManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

