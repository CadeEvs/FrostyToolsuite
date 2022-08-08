using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnlineMatchResultFakeEntityData))]
	public class OnlineMatchResultFakeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnlineMatchResultFakeEntityData>
	{
		public new FrostySdk.Ebx.OnlineMatchResultFakeEntityData Data => data as FrostySdk.Ebx.OnlineMatchResultFakeEntityData;
		public override string DisplayName => "OnlineMatchResultFake";

		public OnlineMatchResultFakeEntity(FrostySdk.Ebx.OnlineMatchResultFakeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

