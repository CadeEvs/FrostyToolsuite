using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeybindingDataProviderData))]
	public class KeybindingDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.KeybindingDataProviderData>
	{
		public new FrostySdk.Ebx.KeybindingDataProviderData Data => data as FrostySdk.Ebx.KeybindingDataProviderData;
		public override string DisplayName => "KeybindingDataProvider";

		public KeybindingDataProvider(FrostySdk.Ebx.KeybindingDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

