using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalizedStringEntityData))]
	public class LocalizedStringEntity : LocalizedStringEntityBase, IEntityData<FrostySdk.Ebx.LocalizedStringEntityData>
	{
		public new FrostySdk.Ebx.LocalizedStringEntityData Data => data as FrostySdk.Ebx.LocalizedStringEntityData;
		public override string DisplayName => "LocalizedString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalizedStringEntity(FrostySdk.Ebx.LocalizedStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

