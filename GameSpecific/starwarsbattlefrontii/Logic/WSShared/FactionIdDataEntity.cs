using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FactionIdDataEntityData))]
	public class FactionIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FactionIdDataEntityData>
	{
		public new FrostySdk.Ebx.FactionIdDataEntityData Data => data as FrostySdk.Ebx.FactionIdDataEntityData;
		public override string DisplayName => "FactionIdData";

		public FactionIdDataEntity(FrostySdk.Ebx.FactionIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

