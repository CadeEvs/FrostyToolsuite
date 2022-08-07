using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectVariationSwitchEntityData))]
	public class ObjectVariationSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectVariationSwitchEntityData>
	{
		public new FrostySdk.Ebx.ObjectVariationSwitchEntityData Data => data as FrostySdk.Ebx.ObjectVariationSwitchEntityData;
		public override string DisplayName => "ObjectVariationSwitch";

		public ObjectVariationSwitchEntity(FrostySdk.Ebx.ObjectVariationSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

