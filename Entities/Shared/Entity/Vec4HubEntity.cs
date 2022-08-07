using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec4HubEntityData))]
	public class Vec4HubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec4HubEntityData>
	{
		public new FrostySdk.Ebx.Vec4HubEntityData Data => data as FrostySdk.Ebx.Vec4HubEntityData;
		public override string DisplayName => "Vec4Hub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec4HubEntity(FrostySdk.Ebx.Vec4HubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

