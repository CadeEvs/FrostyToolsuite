using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QoSResponseListEntityData))]
	public class QoSResponseListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QoSResponseListEntityData>
	{
		public new FrostySdk.Ebx.QoSResponseListEntityData Data => data as FrostySdk.Ebx.QoSResponseListEntityData;
		public override string DisplayName => "QoSResponseList";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QoSResponseListEntity(FrostySdk.Ebx.QoSResponseListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

