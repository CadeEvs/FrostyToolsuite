using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformSpaceEntityData))]
	public class TransformSpaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformSpaceEntityData>
	{
		public new FrostySdk.Ebx.TransformSpaceEntityData Data => data as FrostySdk.Ebx.TransformSpaceEntityData;
		public override string DisplayName => "TransformSpace";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformSpaceEntity(FrostySdk.Ebx.TransformSpaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

