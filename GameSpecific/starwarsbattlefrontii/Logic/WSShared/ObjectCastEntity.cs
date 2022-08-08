using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectCastEntityData))]
	public class ObjectCastEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectCastEntityData>
	{
		public new FrostySdk.Ebx.ObjectCastEntityData Data => data as FrostySdk.Ebx.ObjectCastEntityData;
		public override string DisplayName => "ObjectCast";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ObjectCastEntity(FrostySdk.Ebx.ObjectCastEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

