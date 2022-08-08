using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CodexDataProviderData))]
	public class CodexDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.CodexDataProviderData>
	{
		public new FrostySdk.Ebx.CodexDataProviderData Data => data as FrostySdk.Ebx.CodexDataProviderData;
		public override string DisplayName => "CodexDataProvider";

		public CodexDataProvider(FrostySdk.Ebx.CodexDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

