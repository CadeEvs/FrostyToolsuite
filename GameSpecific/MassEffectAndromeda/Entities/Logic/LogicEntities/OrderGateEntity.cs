using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OrderGateEntityData))]
	public class OrderGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OrderGateEntityData>
	{
		public new FrostySdk.Ebx.OrderGateEntityData Data => data as FrostySdk.Ebx.OrderGateEntityData;
		public override string DisplayName => "OrderGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OrderGateEntity(FrostySdk.Ebx.OrderGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

