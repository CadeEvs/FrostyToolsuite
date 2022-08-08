using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomizationDataEntityData))]
	public class CustomizationDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CustomizationDataEntityData>
	{
		public new FrostySdk.Ebx.CustomizationDataEntityData Data => data as FrostySdk.Ebx.CustomizationDataEntityData;
		public override string DisplayName => "CustomizationData";

		public CustomizationDataEntity(FrostySdk.Ebx.CustomizationDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

