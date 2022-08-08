using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LevelControlEntityData))]
	public class LevelControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LevelControlEntityData>
	{
		public new FrostySdk.Ebx.LevelControlEntityData Data => data as FrostySdk.Ebx.LevelControlEntityData;
		public override string DisplayName => "LevelControl";

		public LevelControlEntity(FrostySdk.Ebx.LevelControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

