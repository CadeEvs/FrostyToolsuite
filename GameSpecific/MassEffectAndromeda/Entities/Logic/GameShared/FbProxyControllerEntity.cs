using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FbProxyControllerEntityData))]
	public class FbProxyControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FbProxyControllerEntityData>
	{
		public new FrostySdk.Ebx.FbProxyControllerEntityData Data => data as FrostySdk.Ebx.FbProxyControllerEntityData;
		public override string DisplayName => "FbProxyController";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FbProxyControllerEntity(FrostySdk.Ebx.FbProxyControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

