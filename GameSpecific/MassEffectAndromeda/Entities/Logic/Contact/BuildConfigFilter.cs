using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BuildConfigFilterData))]
	public class BuildConfigFilter : LogicEntity, IEntityData<FrostySdk.Ebx.BuildConfigFilterData>
	{
		public new FrostySdk.Ebx.BuildConfigFilterData Data => data as FrostySdk.Ebx.BuildConfigFilterData;
		public override string DisplayName => "BuildConfigFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BuildConfigFilter(FrostySdk.Ebx.BuildConfigFilterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

