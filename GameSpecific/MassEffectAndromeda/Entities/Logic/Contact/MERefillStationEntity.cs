using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MERefillStationEntityData))]
	public class MERefillStationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MERefillStationEntityData>
	{
		public new FrostySdk.Ebx.MERefillStationEntityData Data => data as FrostySdk.Ebx.MERefillStationEntityData;
		public override string DisplayName => "MERefillStation";

		public MERefillStationEntity(FrostySdk.Ebx.MERefillStationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

