using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetEnableMaxRegenerationLimitEntityData))]
	public class SetEnableMaxRegenerationLimitEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetEnableMaxRegenerationLimitEntityData>
	{
		public new FrostySdk.Ebx.SetEnableMaxRegenerationLimitEntityData Data => data as FrostySdk.Ebx.SetEnableMaxRegenerationLimitEntityData;
		public override string DisplayName => "SetEnableMaxRegenerationLimit";

		public SetEnableMaxRegenerationLimitEntity(FrostySdk.Ebx.SetEnableMaxRegenerationLimitEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

