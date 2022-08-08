using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MELevelManagerEntityData))]
	public class MELevelManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MELevelManagerEntityData>
	{
		public new FrostySdk.Ebx.MELevelManagerEntityData Data => data as FrostySdk.Ebx.MELevelManagerEntityData;
		public override string DisplayName => "MELevelManager";

		public MELevelManagerEntity(FrostySdk.Ebx.MELevelManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

