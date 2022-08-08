using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CheckedLocalizedStringEntityData))]
	public class CheckedLocalizedStringEntity : LocalizedStringEntityBase, IEntityData<FrostySdk.Ebx.CheckedLocalizedStringEntityData>
	{
		public new FrostySdk.Ebx.CheckedLocalizedStringEntityData Data => data as FrostySdk.Ebx.CheckedLocalizedStringEntityData;
		public override string DisplayName => "CheckedLocalizedString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CheckedLocalizedStringEntity(FrostySdk.Ebx.CheckedLocalizedStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

