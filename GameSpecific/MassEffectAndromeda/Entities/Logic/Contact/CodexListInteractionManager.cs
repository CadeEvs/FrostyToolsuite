using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CodexListInteractionManagerData))]
	public class CodexListInteractionManager : LogicEntity, IEntityData<FrostySdk.Ebx.CodexListInteractionManagerData>
	{
		public new FrostySdk.Ebx.CodexListInteractionManagerData Data => data as FrostySdk.Ebx.CodexListInteractionManagerData;
		public override string DisplayName => "CodexListInteractionManager";

		public CodexListInteractionManager(FrostySdk.Ebx.CodexListInteractionManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

