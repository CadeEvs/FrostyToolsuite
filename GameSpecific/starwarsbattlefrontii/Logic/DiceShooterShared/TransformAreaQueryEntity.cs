using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformAreaQueryEntityData))]
	public class TransformAreaQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformAreaQueryEntityData>
	{
		public new FrostySdk.Ebx.TransformAreaQueryEntityData Data => data as FrostySdk.Ebx.TransformAreaQueryEntityData;
		public override string DisplayName => "TransformAreaQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformAreaQueryEntity(FrostySdk.Ebx.TransformAreaQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

