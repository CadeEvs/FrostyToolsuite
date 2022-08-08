using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WhooshbyPlayerEntityData))]
	public class WhooshbyPlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WhooshbyPlayerEntityData>
	{
		public new FrostySdk.Ebx.WhooshbyPlayerEntityData Data => data as FrostySdk.Ebx.WhooshbyPlayerEntityData;
		public override string DisplayName => "WhooshbyPlayer";

		public WhooshbyPlayerEntity(FrostySdk.Ebx.WhooshbyPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

