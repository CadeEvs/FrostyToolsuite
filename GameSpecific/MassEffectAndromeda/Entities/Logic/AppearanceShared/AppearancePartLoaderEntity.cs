using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearancePartLoaderEntityData))]
	public class AppearancePartLoaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AppearancePartLoaderEntityData>
	{
		public new FrostySdk.Ebx.AppearancePartLoaderEntityData Data => data as FrostySdk.Ebx.AppearancePartLoaderEntityData;
		public override string DisplayName => "AppearancePartLoader";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AppearancePartLoaderEntity(FrostySdk.Ebx.AppearancePartLoaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

