using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalVec3EntityData))]
	public class ConditionalVec3Entity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalVec3EntityData>
	{
		public new FrostySdk.Ebx.ConditionalVec3EntityData Data => data as FrostySdk.Ebx.ConditionalVec3EntityData;
		public override string DisplayName => "ConditionalVec3";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalVec3Entity(FrostySdk.Ebx.ConditionalVec3EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

