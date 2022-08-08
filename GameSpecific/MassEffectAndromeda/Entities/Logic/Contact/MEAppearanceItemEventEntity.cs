using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAppearanceItemEventEntityData))]
	public class MEAppearanceItemEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEAppearanceItemEventEntityData>
	{
		public new FrostySdk.Ebx.MEAppearanceItemEventEntityData Data => data as FrostySdk.Ebx.MEAppearanceItemEventEntityData;
		public override string DisplayName => "MEAppearanceItemEvent";

		public MEAppearanceItemEventEntity(FrostySdk.Ebx.MEAppearanceItemEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

