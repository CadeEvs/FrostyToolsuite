using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EulerTransformSplitterEntityData))]
	public class EulerTransformSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EulerTransformSplitterEntityData>
	{
		public new FrostySdk.Ebx.EulerTransformSplitterEntityData Data => data as FrostySdk.Ebx.EulerTransformSplitterEntityData;
		public override string DisplayName => "EulerTransformSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EulerTransformSplitterEntity(FrostySdk.Ebx.EulerTransformSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

