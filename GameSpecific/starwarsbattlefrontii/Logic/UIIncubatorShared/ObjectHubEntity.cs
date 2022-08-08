using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectHubEntityData))]
	public class ObjectHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectHubEntityData>
	{
		public new FrostySdk.Ebx.ObjectHubEntityData Data => data as FrostySdk.Ebx.ObjectHubEntityData;
		public override string DisplayName => "ObjectHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ObjectHubEntity(FrostySdk.Ebx.ObjectHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

