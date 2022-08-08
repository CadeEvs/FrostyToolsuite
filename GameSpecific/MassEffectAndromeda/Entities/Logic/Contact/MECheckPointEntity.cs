using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECheckPointEntityData))]
	public class MECheckPointEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MECheckPointEntityData>
	{
		public new FrostySdk.Ebx.MECheckPointEntityData Data => data as FrostySdk.Ebx.MECheckPointEntityData;
		public override string DisplayName => "MECheckPoint";

		public MECheckPointEntity(FrostySdk.Ebx.MECheckPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

