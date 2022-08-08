using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FrontendCollectionControlEntityData))]
	public class FrontendCollectionControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FrontendCollectionControlEntityData>
	{
		public new FrostySdk.Ebx.FrontendCollectionControlEntityData Data => data as FrostySdk.Ebx.FrontendCollectionControlEntityData;
		public override string DisplayName => "FrontendCollectionControl";

		public FrontendCollectionControlEntity(FrostySdk.Ebx.FrontendCollectionControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

