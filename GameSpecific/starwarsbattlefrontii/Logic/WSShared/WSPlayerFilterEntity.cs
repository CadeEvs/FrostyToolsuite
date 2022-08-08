using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSPlayerFilterEntityData))]
	public class WSPlayerFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSPlayerFilterEntityData>
	{
		public new FrostySdk.Ebx.WSPlayerFilterEntityData Data => data as FrostySdk.Ebx.WSPlayerFilterEntityData;
		public override string DisplayName => "WSPlayerFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSPlayerFilterEntity(FrostySdk.Ebx.WSPlayerFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

