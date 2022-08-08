using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GateEntityData))]
	public class GateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GateEntityData>
	{
		public new FrostySdk.Ebx.GateEntityData Data => data as FrostySdk.Ebx.GateEntityData;
		public override string DisplayName => "Gate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GateEntity(FrostySdk.Ebx.GateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

