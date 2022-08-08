using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalFloatEntityData))]
	public class ConditionalFloatEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalFloatEntityData>
	{
		public new FrostySdk.Ebx.ConditionalFloatEntityData Data => data as FrostySdk.Ebx.ConditionalFloatEntityData;
		public override string DisplayName => "ConditionalFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalFloatEntity(FrostySdk.Ebx.ConditionalFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

