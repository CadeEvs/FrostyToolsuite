using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestStaticModelGroupEntityData))]
	public class TestStaticModelGroupEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.TestStaticModelGroupEntityData>
	{
		public new FrostySdk.Ebx.TestStaticModelGroupEntityData Data => data as FrostySdk.Ebx.TestStaticModelGroupEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TestStaticModelGroupEntity(FrostySdk.Ebx.TestStaticModelGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

