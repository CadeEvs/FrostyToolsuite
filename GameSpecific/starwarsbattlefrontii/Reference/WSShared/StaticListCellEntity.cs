using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticListCellEntityData))]
	public class StaticListCellEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.StaticListCellEntityData>
	{
		public new FrostySdk.Ebx.StaticListCellEntityData Data => data as FrostySdk.Ebx.StaticListCellEntityData;

		public StaticListCellEntity(FrostySdk.Ebx.StaticListCellEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

