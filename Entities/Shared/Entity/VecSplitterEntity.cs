using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VecSplitterEntityData))]
	public class VecSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VecSplitterEntityData>
	{
		public new FrostySdk.Ebx.VecSplitterEntityData Data => data as FrostySdk.Ebx.VecSplitterEntityData;
		public override string DisplayName => "VecSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VecSplitterEntity(FrostySdk.Ebx.VecSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

