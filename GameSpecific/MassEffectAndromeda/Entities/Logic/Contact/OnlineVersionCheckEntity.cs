using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineVersionCheckEntityData))]
	public class OnlineVersionCheckEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineVersionCheckEntityData>
	{
		public new FrostySdk.Ebx.OnlineVersionCheckEntityData Data => data as FrostySdk.Ebx.OnlineVersionCheckEntityData;
		public override string DisplayName => "OnlineVersionCheck";

		public OnlineVersionCheckEntity(FrostySdk.Ebx.OnlineVersionCheckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

