using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttachSoundsetFilterEntityData))]
	public class AttachSoundsetFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AttachSoundsetFilterEntityData>
	{
		public new FrostySdk.Ebx.AttachSoundsetFilterEntityData Data => data as FrostySdk.Ebx.AttachSoundsetFilterEntityData;
		public override string DisplayName => "AttachSoundsetFilter";

		public AttachSoundsetFilterEntity(FrostySdk.Ebx.AttachSoundsetFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

