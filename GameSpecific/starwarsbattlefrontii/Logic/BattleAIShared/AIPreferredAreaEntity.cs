using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPreferredAreaEntityData))]
	public class AIPreferredAreaEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AIPreferredAreaEntityData>
	{
		public new FrostySdk.Ebx.AIPreferredAreaEntityData Data => data as FrostySdk.Ebx.AIPreferredAreaEntityData;
		public override string DisplayName => "AIPreferredArea";

		public AIPreferredAreaEntity(FrostySdk.Ebx.AIPreferredAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

