using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundsetDirectorEntityData))]
	public class SoundsetDirectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundsetDirectorEntityData>
	{
		public new FrostySdk.Ebx.SoundsetDirectorEntityData Data => data as FrostySdk.Ebx.SoundsetDirectorEntityData;
		public override string DisplayName => "SoundsetDirector";

		public SoundsetDirectorEntity(FrostySdk.Ebx.SoundsetDirectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

