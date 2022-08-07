using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatSelectEntityData))]
	public class FloatSelectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatSelectEntityData>
	{
		public new FrostySdk.Ebx.FloatSelectEntityData Data => data as FrostySdk.Ebx.FloatSelectEntityData;
		public override string DisplayName => "FloatSelect";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatSelectEntity(FrostySdk.Ebx.FloatSelectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

