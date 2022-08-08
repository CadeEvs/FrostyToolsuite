using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BanterUIEntityData))]
	public class BanterUIEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BanterUIEntityData>
	{
		public new FrostySdk.Ebx.BanterUIEntityData Data => data as FrostySdk.Ebx.BanterUIEntityData;
		public override string DisplayName => "BanterUI";

		public BanterUIEntity(FrostySdk.Ebx.BanterUIEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

