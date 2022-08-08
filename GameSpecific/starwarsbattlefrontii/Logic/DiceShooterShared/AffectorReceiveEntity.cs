using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AffectorReceiveEntityData))]
	public class AffectorReceiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AffectorReceiveEntityData>
	{
		public new FrostySdk.Ebx.AffectorReceiveEntityData Data => data as FrostySdk.Ebx.AffectorReceiveEntityData;
		public override string DisplayName => "AffectorReceive";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AffectorReceiveEntity(FrostySdk.Ebx.AffectorReceiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

