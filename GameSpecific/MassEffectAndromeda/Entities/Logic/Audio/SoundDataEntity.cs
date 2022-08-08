using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundDataEntityData))]
	public class SoundDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundDataEntityData>
	{
		public new FrostySdk.Ebx.SoundDataEntityData Data => data as FrostySdk.Ebx.SoundDataEntityData;
		public override string DisplayName => "SoundData";

		public SoundDataEntity(FrostySdk.Ebx.SoundDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

