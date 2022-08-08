using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProxyPlayerSquadEntityData))]
	public class ProxyPlayerSquadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProxyPlayerSquadEntityData>
	{
		public new FrostySdk.Ebx.ProxyPlayerSquadEntityData Data => data as FrostySdk.Ebx.ProxyPlayerSquadEntityData;
		public override string DisplayName => "ProxyPlayerSquad";

		public ProxyPlayerSquadEntity(FrostySdk.Ebx.ProxyPlayerSquadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

