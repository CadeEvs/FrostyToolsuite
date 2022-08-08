using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundHdrEntityData))]
	public class SoundHdrEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundHdrEntityData>
	{
		public new FrostySdk.Ebx.SoundHdrEntityData Data => data as FrostySdk.Ebx.SoundHdrEntityData;
		public override string DisplayName => "SoundHdr";

		public SoundHdrEntity(FrostySdk.Ebx.SoundHdrEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

