using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEBoosterManagerEntityData))]
	public class MEBoosterManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEBoosterManagerEntityData>
	{
		public new FrostySdk.Ebx.MEBoosterManagerEntityData Data => data as FrostySdk.Ebx.MEBoosterManagerEntityData;
		public override string DisplayName => "MEBoosterManager";

		public MEBoosterManagerEntity(FrostySdk.Ebx.MEBoosterManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

