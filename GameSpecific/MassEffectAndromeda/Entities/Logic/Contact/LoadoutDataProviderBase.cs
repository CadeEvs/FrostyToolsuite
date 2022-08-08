using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadoutDataProviderBaseData))]
	public class LoadoutDataProviderBase : MeshSpawner, IEntityData<FrostySdk.Ebx.LoadoutDataProviderBaseData>
	{
		public new FrostySdk.Ebx.LoadoutDataProviderBaseData Data => data as FrostySdk.Ebx.LoadoutDataProviderBaseData;
		public override string DisplayName => "LoadoutDataProviderBase";

		public LoadoutDataProviderBase(FrostySdk.Ebx.LoadoutDataProviderBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

