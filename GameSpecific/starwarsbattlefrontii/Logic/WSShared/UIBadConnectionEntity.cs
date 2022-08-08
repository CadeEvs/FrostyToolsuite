using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIBadConnectionEntityData))]
	public class UIBadConnectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIBadConnectionEntityData>
	{
		public new FrostySdk.Ebx.UIBadConnectionEntityData Data => data as FrostySdk.Ebx.UIBadConnectionEntityData;
		public override string DisplayName => "UIBadConnection";

		public UIBadConnectionEntity(FrostySdk.Ebx.UIBadConnectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

