using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformParameterEventEntityData))]
	public class TransformParameterEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformParameterEventEntityData>
	{
		public new FrostySdk.Ebx.TransformParameterEventEntityData Data => data as FrostySdk.Ebx.TransformParameterEventEntityData;
		public override string DisplayName => "TransformParameterEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformParameterEventEntity(FrostySdk.Ebx.TransformParameterEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

