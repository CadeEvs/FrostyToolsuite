using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineMatchEntityData))]
	public class OnlineMatchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineMatchEntityData>
	{
		public new FrostySdk.Ebx.OnlineMatchEntityData Data => data as FrostySdk.Ebx.OnlineMatchEntityData;
		public override string DisplayName => "OnlineMatch";

		public OnlineMatchEntity(FrostySdk.Ebx.OnlineMatchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

