using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SiblingHeadLookupData))]
	public class SiblingHeadLookup : LogicEntity, IEntityData<FrostySdk.Ebx.SiblingHeadLookupData>
	{
		public new FrostySdk.Ebx.SiblingHeadLookupData Data => data as FrostySdk.Ebx.SiblingHeadLookupData;
		public override string DisplayName => "SiblingHeadLookup";

		public SiblingHeadLookup(FrostySdk.Ebx.SiblingHeadLookupData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

