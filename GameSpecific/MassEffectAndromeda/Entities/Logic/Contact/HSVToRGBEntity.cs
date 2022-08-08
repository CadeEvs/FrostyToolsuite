using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HSVToRGBEntityData))]
	public class HSVToRGBEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HSVToRGBEntityData>
	{
		public new FrostySdk.Ebx.HSVToRGBEntityData Data => data as FrostySdk.Ebx.HSVToRGBEntityData;
		public override string DisplayName => "HSVToRGB";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HSVToRGBEntity(FrostySdk.Ebx.HSVToRGBEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

