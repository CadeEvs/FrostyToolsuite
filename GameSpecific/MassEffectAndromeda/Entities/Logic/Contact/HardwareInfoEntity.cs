using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HardwareInfoEntityData))]
	public class HardwareInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HardwareInfoEntityData>
	{
		public new FrostySdk.Ebx.HardwareInfoEntityData Data => data as FrostySdk.Ebx.HardwareInfoEntityData;
		public override string DisplayName => "HardwareInfo";

		public HardwareInfoEntity(FrostySdk.Ebx.HardwareInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

