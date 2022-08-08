using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundActivityTesterEntityData))]
	public class SoundActivityTesterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundActivityTesterEntityData>
	{
		public new FrostySdk.Ebx.SoundActivityTesterEntityData Data => data as FrostySdk.Ebx.SoundActivityTesterEntityData;
		public override string DisplayName => "SoundActivityTester";

		public SoundActivityTesterEntity(FrostySdk.Ebx.SoundActivityTesterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

