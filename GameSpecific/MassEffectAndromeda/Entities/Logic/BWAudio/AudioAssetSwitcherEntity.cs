using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioAssetSwitcherEntityData))]
	public class AudioAssetSwitcherEntity : AudioAssetEntity, IEntityData<FrostySdk.Ebx.AudioAssetSwitcherEntityData>
	{
		public new FrostySdk.Ebx.AudioAssetSwitcherEntityData Data => data as FrostySdk.Ebx.AudioAssetSwitcherEntityData;
		public override string DisplayName => "AudioAssetSwitcher";

		public AudioAssetSwitcherEntity(FrostySdk.Ebx.AudioAssetSwitcherEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

