using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeybindingsDataProviderData))]
	public class KeybindingsDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.KeybindingsDataProviderData>
	{
		public new FrostySdk.Ebx.KeybindingsDataProviderData Data => data as FrostySdk.Ebx.KeybindingsDataProviderData;
		public override string DisplayName => "KeybindingsDataProvider";

		public KeybindingsDataProvider(FrostySdk.Ebx.KeybindingsDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

