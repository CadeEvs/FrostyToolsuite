using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectVariationMultiSwitchEntityData))]
	public class ObjectVariationMultiSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectVariationMultiSwitchEntityData>
	{
		public new FrostySdk.Ebx.ObjectVariationMultiSwitchEntityData Data => data as FrostySdk.Ebx.ObjectVariationMultiSwitchEntityData;
		public override string DisplayName => "ObjectVariationMultiSwitch";

		public ObjectVariationMultiSwitchEntity(FrostySdk.Ebx.ObjectVariationMultiSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

