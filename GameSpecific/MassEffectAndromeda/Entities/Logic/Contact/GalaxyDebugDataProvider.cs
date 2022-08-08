using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GalaxyDebugDataProviderData))]
	public class GalaxyDebugDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.GalaxyDebugDataProviderData>
	{
		public new FrostySdk.Ebx.GalaxyDebugDataProviderData Data => data as FrostySdk.Ebx.GalaxyDebugDataProviderData;
		public override string DisplayName => "GalaxyDebugDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GalaxyDebugDataProvider(FrostySdk.Ebx.GalaxyDebugDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

