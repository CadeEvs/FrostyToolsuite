using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CodexPopUpEntityData))]
	public class CodexPopUpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CodexPopUpEntityData>
	{
		public new FrostySdk.Ebx.CodexPopUpEntityData Data => data as FrostySdk.Ebx.CodexPopUpEntityData;
		public override string DisplayName => "CodexPopUp";

		public CodexPopUpEntity(FrostySdk.Ebx.CodexPopUpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

