using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICharacterLockEntityData))]
	public class UICharacterLockEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICharacterLockEntityData>
	{
		public new FrostySdk.Ebx.UICharacterLockEntityData Data => data as FrostySdk.Ebx.UICharacterLockEntityData;
		public override string DisplayName => "UICharacterLock";

		public UICharacterLockEntity(FrostySdk.Ebx.UICharacterLockEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

