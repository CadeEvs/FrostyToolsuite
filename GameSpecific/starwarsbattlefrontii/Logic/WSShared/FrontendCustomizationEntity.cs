using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FrontendCustomizationEntityData))]
	public class FrontendCustomizationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FrontendCustomizationEntityData>
	{
		public new FrostySdk.Ebx.FrontendCustomizationEntityData Data => data as FrostySdk.Ebx.FrontendCustomizationEntityData;
		public override string DisplayName => "FrontendCustomization";

		public FrontendCustomizationEntity(FrostySdk.Ebx.FrontendCustomizationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

