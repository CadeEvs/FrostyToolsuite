using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationProxyBankData))]
	public class AnimationProxyBank : LogicEntity, IEntityData<FrostySdk.Ebx.AnimationProxyBankData>
	{
		public new FrostySdk.Ebx.AnimationProxyBankData Data => data as FrostySdk.Ebx.AnimationProxyBankData;
		public override string DisplayName => "AnimationProxyBank";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AnimationProxyBank(FrostySdk.Ebx.AnimationProxyBankData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

