using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldStateControlEntityData))]
	public class WorldStateControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WorldStateControlEntityData>
	{
		public new FrostySdk.Ebx.WorldStateControlEntityData Data => data as FrostySdk.Ebx.WorldStateControlEntityData;
		public override string DisplayName => "WorldStateControl";

		public WorldStateControlEntity(FrostySdk.Ebx.WorldStateControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

