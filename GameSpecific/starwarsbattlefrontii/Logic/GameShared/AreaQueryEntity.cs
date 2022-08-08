using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AreaQueryEntityData))]
	public class AreaQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AreaQueryEntityData>
	{
		public new FrostySdk.Ebx.AreaQueryEntityData Data => data as FrostySdk.Ebx.AreaQueryEntityData;
		public override string DisplayName => "AreaQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AreaQueryEntity(FrostySdk.Ebx.AreaQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

