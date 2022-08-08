using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientLocationSwitchEntityData))]
	public class AmbientLocationSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AmbientLocationSwitchEntityData>
	{
		public new FrostySdk.Ebx.AmbientLocationSwitchEntityData Data => data as FrostySdk.Ebx.AmbientLocationSwitchEntityData;
		public override string DisplayName => "AmbientLocationSwitch";

		public AmbientLocationSwitchEntity(FrostySdk.Ebx.AmbientLocationSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

