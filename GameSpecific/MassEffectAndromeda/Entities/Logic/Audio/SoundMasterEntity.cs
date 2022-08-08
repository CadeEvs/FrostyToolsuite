using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundMasterEntityData))]
	public class SoundMasterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundMasterEntityData>
	{
		public new FrostySdk.Ebx.SoundMasterEntityData Data => data as FrostySdk.Ebx.SoundMasterEntityData;
		public override string DisplayName => "SoundMaster";

		public SoundMasterEntity(FrostySdk.Ebx.SoundMasterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

