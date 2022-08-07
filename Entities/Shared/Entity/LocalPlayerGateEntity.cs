using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerGateEntityData))]
	public class LocalPlayerGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerGateEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerGateEntityData Data => data as FrostySdk.Ebx.LocalPlayerGateEntityData;
		public override string DisplayName => "LocalPlayerGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalPlayerGateEntity(FrostySdk.Ebx.LocalPlayerGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

