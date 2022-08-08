using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalizedStringEntityData))]
	public class LocalizedStringEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalizedStringEntityData>
	{
		public new FrostySdk.Ebx.LocalizedStringEntityData Data => data as FrostySdk.Ebx.LocalizedStringEntityData;
		public override string DisplayName => "LocalizedString";

		public LocalizedStringEntity(FrostySdk.Ebx.LocalizedStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

