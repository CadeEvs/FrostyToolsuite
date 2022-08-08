using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FilterEntityBaseData))]
	public class FilterEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.FilterEntityBaseData>
	{
		public new FrostySdk.Ebx.FilterEntityBaseData Data => data as FrostySdk.Ebx.FilterEntityBaseData;
		public override string DisplayName => "FilterEntityBase";

		public FilterEntityBase(FrostySdk.Ebx.FilterEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

