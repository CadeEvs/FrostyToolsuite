using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CodexListDataProviderData))]
	public class CodexListDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.CodexListDataProviderData>
	{
		public new FrostySdk.Ebx.CodexListDataProviderData Data => data as FrostySdk.Ebx.CodexListDataProviderData;
		public override string DisplayName => "CodexListDataProvider";

		public CodexListDataProvider(FrostySdk.Ebx.CodexListDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

