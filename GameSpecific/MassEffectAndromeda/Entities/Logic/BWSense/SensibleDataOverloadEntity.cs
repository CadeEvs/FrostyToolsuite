using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SensibleDataOverloadEntityData))]
	public class SensibleDataOverloadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SensibleDataOverloadEntityData>
	{
		public new FrostySdk.Ebx.SensibleDataOverloadEntityData Data => data as FrostySdk.Ebx.SensibleDataOverloadEntityData;
		public override string DisplayName => "SensibleDataOverload";

		public SensibleDataOverloadEntity(FrostySdk.Ebx.SensibleDataOverloadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

