using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MultiPlayerLoadoutDataProviderData))]
	public class MultiPlayerLoadoutDataProvider : OnlineLoadoutDataProvider, IEntityData<FrostySdk.Ebx.MultiPlayerLoadoutDataProviderData>
	{
		public new FrostySdk.Ebx.MultiPlayerLoadoutDataProviderData Data => data as FrostySdk.Ebx.MultiPlayerLoadoutDataProviderData;
		public override string DisplayName => "MultiPlayerLoadoutDataProvider";

		public MultiPlayerLoadoutDataProvider(FrostySdk.Ebx.MultiPlayerLoadoutDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

