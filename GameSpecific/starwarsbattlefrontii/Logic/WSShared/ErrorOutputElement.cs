using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ErrorOutputElementData))]
	public class ErrorOutputElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ErrorOutputElementData>
	{
		public new FrostySdk.Ebx.ErrorOutputElementData Data => data as FrostySdk.Ebx.ErrorOutputElementData;
		public override string DisplayName => "ErrorOutputElement";

		public ErrorOutputElement(FrostySdk.Ebx.ErrorOutputElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

