using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GiveXPEntityData))]
	public class GiveXPEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GiveXPEntityData>
	{
		public new FrostySdk.Ebx.GiveXPEntityData Data => data as FrostySdk.Ebx.GiveXPEntityData;
		public override string DisplayName => "GiveXP";

		public GiveXPEntity(FrostySdk.Ebx.GiveXPEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

