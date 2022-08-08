using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticTabListCellEntityData))]
	public class StaticTabListCellEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.StaticTabListCellEntityData>
	{
		public new FrostySdk.Ebx.StaticTabListCellEntityData Data => data as FrostySdk.Ebx.StaticTabListCellEntityData;

		public StaticTabListCellEntity(FrostySdk.Ebx.StaticTabListCellEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

