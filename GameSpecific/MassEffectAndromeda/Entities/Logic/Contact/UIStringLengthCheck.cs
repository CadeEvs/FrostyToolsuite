using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStringLengthCheckData))]
	public class UIStringLengthCheck : LogicEntity, IEntityData<FrostySdk.Ebx.UIStringLengthCheckData>
	{
		public new FrostySdk.Ebx.UIStringLengthCheckData Data => data as FrostySdk.Ebx.UIStringLengthCheckData;
		public override string DisplayName => "UIStringLengthCheck";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UIStringLengthCheck(FrostySdk.Ebx.UIStringLengthCheckData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

