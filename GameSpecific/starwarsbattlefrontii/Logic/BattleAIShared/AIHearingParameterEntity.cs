using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIHearingParameterEntityData))]
	public class AIHearingParameterEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AIHearingParameterEntityData>
	{
		public new FrostySdk.Ebx.AIHearingParameterEntityData Data => data as FrostySdk.Ebx.AIHearingParameterEntityData;
		public override string DisplayName => "AIHearingParameter";

		public AIHearingParameterEntity(FrostySdk.Ebx.AIHearingParameterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

