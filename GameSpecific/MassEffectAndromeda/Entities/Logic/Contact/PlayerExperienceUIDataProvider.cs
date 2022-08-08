using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerExperienceUIDataProviderData))]
	public class PlayerExperienceUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerExperienceUIDataProviderData>
	{
		public new FrostySdk.Ebx.PlayerExperienceUIDataProviderData Data => data as FrostySdk.Ebx.PlayerExperienceUIDataProviderData;
		public override string DisplayName => "PlayerExperienceUIDataProvider";

		public PlayerExperienceUIDataProvider(FrostySdk.Ebx.PlayerExperienceUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

