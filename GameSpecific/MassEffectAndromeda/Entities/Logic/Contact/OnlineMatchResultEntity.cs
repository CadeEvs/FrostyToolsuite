using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineMatchResultEntityData))]
	public class OnlineMatchResultEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineMatchResultEntityData>
	{
		public new FrostySdk.Ebx.OnlineMatchResultEntityData Data => data as FrostySdk.Ebx.OnlineMatchResultEntityData;
		public override string DisplayName => "OnlineMatchResult";

		public OnlineMatchResultEntity(FrostySdk.Ebx.OnlineMatchResultEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

