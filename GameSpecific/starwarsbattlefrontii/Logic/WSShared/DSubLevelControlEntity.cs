using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DSubLevelControlEntityData))]
	public class DSubLevelControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DSubLevelControlEntityData>
	{
		public new FrostySdk.Ebx.DSubLevelControlEntityData Data => data as FrostySdk.Ebx.DSubLevelControlEntityData;
		public override string DisplayName => "DSubLevelControl";

		public DSubLevelControlEntity(FrostySdk.Ebx.DSubLevelControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

