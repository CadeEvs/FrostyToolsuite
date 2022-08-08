using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIOrderEntityBaseData))]
	public class AIOrderEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.AIOrderEntityBaseData>
	{
		public new FrostySdk.Ebx.AIOrderEntityBaseData Data => data as FrostySdk.Ebx.AIOrderEntityBaseData;
		public override string DisplayName => "AIOrderEntityBase";

		public AIOrderEntityBase(FrostySdk.Ebx.AIOrderEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

