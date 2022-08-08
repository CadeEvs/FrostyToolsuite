using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpottingEntityData))]
	public class SpottingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpottingEntityData>
	{
		public new FrostySdk.Ebx.SpottingEntityData Data => data as FrostySdk.Ebx.SpottingEntityData;
		public override string DisplayName => "Spotting";

		public SpottingEntity(FrostySdk.Ebx.SpottingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

