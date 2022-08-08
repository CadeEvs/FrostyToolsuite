using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalVec2EntityData))]
	public class ConditionalVec2Entity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalVec2EntityData>
	{
		public new FrostySdk.Ebx.ConditionalVec2EntityData Data => data as FrostySdk.Ebx.ConditionalVec2EntityData;
		public override string DisplayName => "ConditionalVec2";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalVec2Entity(FrostySdk.Ebx.ConditionalVec2EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

