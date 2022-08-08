using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsFactionIdDataEntityData))]
	public class ContainsFactionIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsFactionIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsFactionIdDataEntityData Data => data as FrostySdk.Ebx.ContainsFactionIdDataEntityData;
		public override string DisplayName => "ContainsFactionIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsFactionIdDataEntity(FrostySdk.Ebx.ContainsFactionIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

