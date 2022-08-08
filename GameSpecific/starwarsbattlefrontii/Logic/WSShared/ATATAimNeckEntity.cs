using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ATATAimNeckEntityData))]
	public class ATATAimNeckEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ATATAimNeckEntityData>
	{
		public new FrostySdk.Ebx.ATATAimNeckEntityData Data => data as FrostySdk.Ebx.ATATAimNeckEntityData;
		public override string DisplayName => "ATATAimNeck";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ATATAimNeckEntity(FrostySdk.Ebx.ATATAimNeckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

