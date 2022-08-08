using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RopeEntityData))]
	public class RopeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RopeEntityData>
	{
		public new FrostySdk.Ebx.RopeEntityData Data => data as FrostySdk.Ebx.RopeEntityData;
		public override string DisplayName => "Rope";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RopeEntity(FrostySdk.Ebx.RopeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

