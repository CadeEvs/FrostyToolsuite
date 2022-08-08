using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalIntEntityData))]
	public class ConditionalIntEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalIntEntityData>
	{
		public new FrostySdk.Ebx.ConditionalIntEntityData Data => data as FrostySdk.Ebx.ConditionalIntEntityData;
		public override string DisplayName => "ConditionalInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalIntEntity(FrostySdk.Ebx.ConditionalIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

