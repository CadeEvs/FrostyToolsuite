using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlinePlayerCardProfileEntityData))]
	public class OnlinePlayerCardProfileEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlinePlayerCardProfileEntityData>
	{
		public new FrostySdk.Ebx.OnlinePlayerCardProfileEntityData Data => data as FrostySdk.Ebx.OnlinePlayerCardProfileEntityData;
		public override string DisplayName => "OnlinePlayerCardProfile";

		public OnlinePlayerCardProfileEntity(FrostySdk.Ebx.OnlinePlayerCardProfileEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

