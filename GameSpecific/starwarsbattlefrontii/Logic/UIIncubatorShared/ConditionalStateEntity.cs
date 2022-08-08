using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalStateEntityData))]
	public class ConditionalStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConditionalStateEntityData>
	{
		public new FrostySdk.Ebx.ConditionalStateEntityData Data => data as FrostySdk.Ebx.ConditionalStateEntityData;
		public override string DisplayName => "ConditionalState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalStateEntity(FrostySdk.Ebx.ConditionalStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

