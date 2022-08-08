using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundAreaEntityData))]
	public class SoundAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundAreaEntityData>
	{
		public new FrostySdk.Ebx.SoundAreaEntityData Data => data as FrostySdk.Ebx.SoundAreaEntityData;
		public override string DisplayName => "SoundArea";

		public SoundAreaEntity(FrostySdk.Ebx.SoundAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

