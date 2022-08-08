using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualUnlockSkinEntityData))]
	public class VisualUnlockSkinEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VisualUnlockSkinEntityData>
	{
		public new FrostySdk.Ebx.VisualUnlockSkinEntityData Data => data as FrostySdk.Ebx.VisualUnlockSkinEntityData;
		public override string DisplayName => "VisualUnlockSkin";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VisualUnlockSkinEntity(FrostySdk.Ebx.VisualUnlockSkinEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

