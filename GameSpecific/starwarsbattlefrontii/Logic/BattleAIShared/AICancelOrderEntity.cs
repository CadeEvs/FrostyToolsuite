using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICancelOrderEntityData))]
	public class AICancelOrderEntity : AIOrderEntityBase, IEntityData<FrostySdk.Ebx.AICancelOrderEntityData>
	{
		public new FrostySdk.Ebx.AICancelOrderEntityData Data => data as FrostySdk.Ebx.AICancelOrderEntityData;
		public override string DisplayName => "AICancelOrder";

		public AICancelOrderEntity(FrostySdk.Ebx.AICancelOrderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

