using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerKeyedValueEntityData))]
	public class PlayerKeyedValueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerKeyedValueEntityData>
	{
		public new FrostySdk.Ebx.PlayerKeyedValueEntityData Data => data as FrostySdk.Ebx.PlayerKeyedValueEntityData;
		public override string DisplayName => "PlayerKeyedValue";

		public PlayerKeyedValueEntity(FrostySdk.Ebx.PlayerKeyedValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

