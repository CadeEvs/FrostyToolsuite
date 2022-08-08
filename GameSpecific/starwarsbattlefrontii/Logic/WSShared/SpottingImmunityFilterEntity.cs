using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpottingImmunityFilterEntityData))]
	public class SpottingImmunityFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpottingImmunityFilterEntityData>
	{
		public new FrostySdk.Ebx.SpottingImmunityFilterEntityData Data => data as FrostySdk.Ebx.SpottingImmunityFilterEntityData;
		public override string DisplayName => "SpottingImmunityFilter";

		public SpottingImmunityFilterEntity(FrostySdk.Ebx.SpottingImmunityFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

