using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioLocatorEntityData))]
	public class AudioLocatorEntity : LocatorEntity, IEntityData<FrostySdk.Ebx.AudioLocatorEntityData>
	{
		public new FrostySdk.Ebx.AudioLocatorEntityData Data => data as FrostySdk.Ebx.AudioLocatorEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AudioLocatorEntity(FrostySdk.Ebx.AudioLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

