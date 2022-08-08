using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAutoSaveStatusEntityData))]
	public class MEAutoSaveStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEAutoSaveStatusEntityData>
	{
		public new FrostySdk.Ebx.MEAutoSaveStatusEntityData Data => data as FrostySdk.Ebx.MEAutoSaveStatusEntityData;
		public override string DisplayName => "MEAutoSaveStatus";

		public MEAutoSaveStatusEntity(FrostySdk.Ebx.MEAutoSaveStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

