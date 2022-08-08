using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectAreaQueryEntityData))]
	public class ObjectAreaQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectAreaQueryEntityData>
	{
		public new FrostySdk.Ebx.ObjectAreaQueryEntityData Data => data as FrostySdk.Ebx.ObjectAreaQueryEntityData;
		public override string DisplayName => "ObjectAreaQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ObjectAreaQueryEntity(FrostySdk.Ebx.ObjectAreaQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

