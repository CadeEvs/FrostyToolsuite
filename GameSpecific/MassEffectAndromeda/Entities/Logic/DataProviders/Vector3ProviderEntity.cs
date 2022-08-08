using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vector3ProviderEntityData))]
	public class Vector3ProviderEntity : ProviderEntity, IEntityData<FrostySdk.Ebx.Vector3ProviderEntityData>
	{
		public new FrostySdk.Ebx.Vector3ProviderEntityData Data => data as FrostySdk.Ebx.Vector3ProviderEntityData;
		public override string DisplayName => "Vector3Provider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vector3ProviderEntity(FrostySdk.Ebx.Vector3ProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

