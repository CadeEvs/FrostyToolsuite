using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoolDownGateEntityData))]
	public class CoolDownGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoolDownGateEntityData>
	{
		public new FrostySdk.Ebx.CoolDownGateEntityData Data => data as FrostySdk.Ebx.CoolDownGateEntityData;
		public override string DisplayName => "CoolDownGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CoolDownGateEntity(FrostySdk.Ebx.CoolDownGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

