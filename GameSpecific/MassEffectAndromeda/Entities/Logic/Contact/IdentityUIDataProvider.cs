using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IdentityUIDataProviderData))]
	public class IdentityUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.IdentityUIDataProviderData>
	{
		public new FrostySdk.Ebx.IdentityUIDataProviderData Data => data as FrostySdk.Ebx.IdentityUIDataProviderData;
		public override string DisplayName => "IdentityUIDataProvider";

		public IdentityUIDataProvider(FrostySdk.Ebx.IdentityUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

