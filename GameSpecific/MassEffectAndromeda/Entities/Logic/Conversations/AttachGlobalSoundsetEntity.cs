using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttachGlobalSoundsetEntityData))]
	public class AttachGlobalSoundsetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AttachGlobalSoundsetEntityData>
	{
		public new FrostySdk.Ebx.AttachGlobalSoundsetEntityData Data => data as FrostySdk.Ebx.AttachGlobalSoundsetEntityData;
		public override string DisplayName => "AttachGlobalSoundset";

		public AttachGlobalSoundsetEntity(FrostySdk.Ebx.AttachGlobalSoundsetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

