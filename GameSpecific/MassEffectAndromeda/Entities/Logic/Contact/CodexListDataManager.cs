using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CodexListDataManagerData))]
	public class CodexListDataManager : SingletonEntity, IEntityData<FrostySdk.Ebx.CodexListDataManagerData>
	{
		public new FrostySdk.Ebx.CodexListDataManagerData Data => data as FrostySdk.Ebx.CodexListDataManagerData;
		public override string DisplayName => "CodexListDataManager";

		public CodexListDataManager(FrostySdk.Ebx.CodexListDataManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

