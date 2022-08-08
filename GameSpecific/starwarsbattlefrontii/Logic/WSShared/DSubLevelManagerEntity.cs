using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DSubLevelManagerEntityData))]
	public class DSubLevelManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DSubLevelManagerEntityData>
	{
		public new FrostySdk.Ebx.DSubLevelManagerEntityData Data => data as FrostySdk.Ebx.DSubLevelManagerEntityData;
		public override string DisplayName => "DSubLevelManager";

		public DSubLevelManagerEntity(FrostySdk.Ebx.DSubLevelManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

