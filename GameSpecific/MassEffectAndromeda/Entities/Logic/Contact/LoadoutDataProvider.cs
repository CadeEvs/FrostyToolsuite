using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadoutDataProviderData))]
	public class LoadoutDataProvider : LoadoutDataProviderBase, IEntityData<FrostySdk.Ebx.LoadoutDataProviderData>
	{
		public new FrostySdk.Ebx.LoadoutDataProviderData Data => data as FrostySdk.Ebx.LoadoutDataProviderData;
		public override string DisplayName => "LoadoutDataProvider";

		public LoadoutDataProvider(FrostySdk.Ebx.LoadoutDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

