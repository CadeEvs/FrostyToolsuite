using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIFriendlyAreaEntityData))]
	public class AIFriendlyAreaEntity : AIParameterWithShapeEntity, IEntityData<FrostySdk.Ebx.AIFriendlyAreaEntityData>
	{
		public new FrostySdk.Ebx.AIFriendlyAreaEntityData Data => data as FrostySdk.Ebx.AIFriendlyAreaEntityData;
		public override string DisplayName => "AIFriendlyArea";

		public AIFriendlyAreaEntity(FrostySdk.Ebx.AIFriendlyAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

