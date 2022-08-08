using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIGotoPlaceOrderEntityData))]
	public class AIGotoPlaceOrderEntity : AIOrderEntityBase, IEntityData<FrostySdk.Ebx.AIGotoPlaceOrderEntityData>
	{
		public new FrostySdk.Ebx.AIGotoPlaceOrderEntityData Data => data as FrostySdk.Ebx.AIGotoPlaceOrderEntityData;
		public override string DisplayName => "AIGotoPlaceOrder";

		public AIGotoPlaceOrderEntity(FrostySdk.Ebx.AIGotoPlaceOrderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

