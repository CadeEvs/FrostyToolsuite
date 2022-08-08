using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatStateEntityData))]
	public class FloatStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatStateEntityData>
	{
		public new FrostySdk.Ebx.FloatStateEntityData Data => data as FrostySdk.Ebx.FloatStateEntityData;
		public override string DisplayName => "FloatState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatStateEntity(FrostySdk.Ebx.FloatStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

