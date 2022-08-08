using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HumanPlayerEntityData))]
	public class HumanPlayerEntity : HumanPlayerProxyEntity, IEntityData<FrostySdk.Ebx.HumanPlayerEntityData>
	{
		public new FrostySdk.Ebx.HumanPlayerEntityData Data => data as FrostySdk.Ebx.HumanPlayerEntityData;
		public override string DisplayName => "HumanPlayer";

		public HumanPlayerEntity(FrostySdk.Ebx.HumanPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

