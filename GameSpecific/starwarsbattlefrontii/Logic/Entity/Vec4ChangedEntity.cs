using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec4ChangedEntityData))]
	public class Vec4ChangedEntity : PropertyChangedEntity, IEntityData<FrostySdk.Ebx.Vec4ChangedEntityData>
	{
		public new FrostySdk.Ebx.Vec4ChangedEntityData Data => data as FrostySdk.Ebx.Vec4ChangedEntityData;
		public override string DisplayName => "Vec4Changed";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vec4ChangedEntity(FrostySdk.Ebx.Vec4ChangedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

