using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FactionInfoEntityData))]
	public class FactionInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FactionInfoEntityData>
	{
		public new FrostySdk.Ebx.FactionInfoEntityData Data => data as FrostySdk.Ebx.FactionInfoEntityData;
		public override string DisplayName => "FactionInfo";

		public FactionInfoEntity(FrostySdk.Ebx.FactionInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

