using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DSubLevelSPInfoQueryData))]
	public class DSubLevelSPInfoQuery : LogicEntity, IEntityData<FrostySdk.Ebx.DSubLevelSPInfoQueryData>
	{
		public new FrostySdk.Ebx.DSubLevelSPInfoQueryData Data => data as FrostySdk.Ebx.DSubLevelSPInfoQueryData;
		public override string DisplayName => "DSubLevelSPInfoQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DSubLevelSPInfoQuery(FrostySdk.Ebx.DSubLevelSPInfoQueryData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

