using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoipDataProviderData))]
	public class VoipDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.VoipDataProviderData>
	{
		public new FrostySdk.Ebx.VoipDataProviderData Data => data as FrostySdk.Ebx.VoipDataProviderData;
		public override string DisplayName => "VoipDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VoipDataProvider(FrostySdk.Ebx.VoipDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

