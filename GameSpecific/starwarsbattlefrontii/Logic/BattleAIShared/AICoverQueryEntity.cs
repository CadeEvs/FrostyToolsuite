using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AICoverQueryEntityData))]
	public class AICoverQueryEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AICoverQueryEntityData>
	{
		public new FrostySdk.Ebx.AICoverQueryEntityData Data => data as FrostySdk.Ebx.AICoverQueryEntityData;
		public override string DisplayName => "AICoverQuery";

		public AICoverQueryEntity(FrostySdk.Ebx.AICoverQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

