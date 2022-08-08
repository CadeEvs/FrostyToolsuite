using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HasNewProgressionDataEntityData))]
	public class HasNewProgressionDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HasNewProgressionDataEntityData>
	{
		public new FrostySdk.Ebx.HasNewProgressionDataEntityData Data => data as FrostySdk.Ebx.HasNewProgressionDataEntityData;
		public override string DisplayName => "HasNewProgressionData";

		public HasNewProgressionDataEntity(FrostySdk.Ebx.HasNewProgressionDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

