using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemDataInfoEntityData))]
	public class SystemDataInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SystemDataInfoEntityData>
	{
		public new FrostySdk.Ebx.SystemDataInfoEntityData Data => data as FrostySdk.Ebx.SystemDataInfoEntityData;
		public override string DisplayName => "SystemDataInfo";

		public SystemDataInfoEntity(FrostySdk.Ebx.SystemDataInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

