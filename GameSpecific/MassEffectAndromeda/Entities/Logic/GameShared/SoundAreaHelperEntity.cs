using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundAreaHelperEntityData))]
	public class SoundAreaHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundAreaHelperEntityData>
	{
		public new FrostySdk.Ebx.SoundAreaHelperEntityData Data => data as FrostySdk.Ebx.SoundAreaHelperEntityData;
		public override string DisplayName => "SoundAreaHelper";

		public SoundAreaHelperEntity(FrostySdk.Ebx.SoundAreaHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

