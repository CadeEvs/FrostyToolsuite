using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSObjectHubEntityData))]
	public class WSObjectHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSObjectHubEntityData>
	{
		public new FrostySdk.Ebx.WSObjectHubEntityData Data => data as FrostySdk.Ebx.WSObjectHubEntityData;
		public override string DisplayName => "WSObjectHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSObjectHubEntity(FrostySdk.Ebx.WSObjectHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

