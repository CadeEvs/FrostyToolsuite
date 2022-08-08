using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MPLevelStateEntityData))]
	public class MPLevelStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MPLevelStateEntityData>
	{
		public new FrostySdk.Ebx.MPLevelStateEntityData Data => data as FrostySdk.Ebx.MPLevelStateEntityData;
		public override string DisplayName => "MPLevelState";

		public MPLevelStateEntity(FrostySdk.Ebx.MPLevelStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

