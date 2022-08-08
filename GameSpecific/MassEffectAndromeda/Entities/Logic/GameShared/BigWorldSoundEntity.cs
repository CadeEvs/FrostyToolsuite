using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BigWorldSoundEntityData))]
	public class BigWorldSoundEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BigWorldSoundEntityData>
	{
		public new FrostySdk.Ebx.BigWorldSoundEntityData Data => data as FrostySdk.Ebx.BigWorldSoundEntityData;
		public override string DisplayName => "BigWorldSound";

		public BigWorldSoundEntity(FrostySdk.Ebx.BigWorldSoundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

