using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEManualSaveControlEntityData))]
	public class MEManualSaveControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEManualSaveControlEntityData>
	{
		public new FrostySdk.Ebx.MEManualSaveControlEntityData Data => data as FrostySdk.Ebx.MEManualSaveControlEntityData;
		public override string DisplayName => "MEManualSaveControl";

		public MEManualSaveControlEntity(FrostySdk.Ebx.MEManualSaveControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

