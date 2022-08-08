using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GlobalSoundsetTriggerEntityData))]
	public class GlobalSoundsetTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GlobalSoundsetTriggerEntityData>
	{
		public new FrostySdk.Ebx.GlobalSoundsetTriggerEntityData Data => data as FrostySdk.Ebx.GlobalSoundsetTriggerEntityData;
		public override string DisplayName => "GlobalSoundsetTrigger";

		public GlobalSoundsetTriggerEntity(FrostySdk.Ebx.GlobalSoundsetTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

