using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISplitscreenInputConfigurationEntityData))]
	public class UISplitscreenInputConfigurationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UISplitscreenInputConfigurationEntityData>
	{
		public new FrostySdk.Ebx.UISplitscreenInputConfigurationEntityData Data => data as FrostySdk.Ebx.UISplitscreenInputConfigurationEntityData;
		public override string DisplayName => "UISplitscreenInputConfiguration";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UISplitscreenInputConfigurationEntity(FrostySdk.Ebx.UISplitscreenInputConfigurationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

