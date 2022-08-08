using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityData))]
	public class SoundEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundEntityData>
	{
		public new FrostySdk.Ebx.SoundEntityData Data => data as FrostySdk.Ebx.SoundEntityData;
		public override string DisplayName => "Sound";

		public SoundEntity(FrostySdk.Ebx.SoundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

