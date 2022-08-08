using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RetailCommandEntityData))]
	public class RetailCommandEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RetailCommandEntityData>
	{
		public new FrostySdk.Ebx.RetailCommandEntityData Data => data as FrostySdk.Ebx.RetailCommandEntityData;
		public override string DisplayName => "RetailCommand";

		public RetailCommandEntity(FrostySdk.Ebx.RetailCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

