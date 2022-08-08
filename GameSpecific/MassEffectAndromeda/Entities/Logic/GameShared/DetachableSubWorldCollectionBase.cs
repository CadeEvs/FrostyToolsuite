using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DetachableSubWorldCollectionBaseData))]
	public class DetachableSubWorldCollectionBase : LogicEntity, IEntityData<FrostySdk.Ebx.DetachableSubWorldCollectionBaseData>
	{
		public new FrostySdk.Ebx.DetachableSubWorldCollectionBaseData Data => data as FrostySdk.Ebx.DetachableSubWorldCollectionBaseData;
		public override string DisplayName => "DetachableSubWorldCollectionBase";

		public DetachableSubWorldCollectionBase(FrostySdk.Ebx.DetachableSubWorldCollectionBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

