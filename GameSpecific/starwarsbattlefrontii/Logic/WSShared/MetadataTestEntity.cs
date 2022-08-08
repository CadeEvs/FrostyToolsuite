using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MetadataTestEntityData))]
	public class MetadataTestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MetadataTestEntityData>
	{
		public new FrostySdk.Ebx.MetadataTestEntityData Data => data as FrostySdk.Ebx.MetadataTestEntityData;
		public override string DisplayName => "MetadataTest";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MetadataTestEntity(FrostySdk.Ebx.MetadataTestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

