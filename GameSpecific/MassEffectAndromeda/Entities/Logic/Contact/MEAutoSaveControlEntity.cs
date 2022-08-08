using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAutoSaveControlEntityData))]
	public class MEAutoSaveControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEAutoSaveControlEntityData>
	{
		public new FrostySdk.Ebx.MEAutoSaveControlEntityData Data => data as FrostySdk.Ebx.MEAutoSaveControlEntityData;
		public override string DisplayName => "MEAutoSaveControl";

		public MEAutoSaveControlEntity(FrostySdk.Ebx.MEAutoSaveControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

