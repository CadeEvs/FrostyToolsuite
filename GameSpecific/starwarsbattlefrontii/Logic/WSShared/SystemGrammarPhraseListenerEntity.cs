using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemGrammarPhraseListenerEntityData))]
	public class SystemGrammarPhraseListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SystemGrammarPhraseListenerEntityData>
	{
		public new FrostySdk.Ebx.SystemGrammarPhraseListenerEntityData Data => data as FrostySdk.Ebx.SystemGrammarPhraseListenerEntityData;
		public override string DisplayName => "SystemGrammarPhraseListener";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SystemGrammarPhraseListenerEntity(FrostySdk.Ebx.SystemGrammarPhraseListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

