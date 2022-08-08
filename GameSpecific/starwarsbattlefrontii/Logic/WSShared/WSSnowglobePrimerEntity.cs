using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSnowglobePrimerEntityData))]
	public class WSSnowglobePrimerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSSnowglobePrimerEntityData>
	{
		public new FrostySdk.Ebx.WSSnowglobePrimerEntityData Data => data as FrostySdk.Ebx.WSSnowglobePrimerEntityData;
		public override string DisplayName => "WSSnowglobePrimer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSSnowglobePrimerEntity(FrostySdk.Ebx.WSSnowglobePrimerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

