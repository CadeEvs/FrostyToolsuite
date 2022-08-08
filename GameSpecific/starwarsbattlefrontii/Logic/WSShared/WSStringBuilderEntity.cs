using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSStringBuilderEntityData))]
	public class WSStringBuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSStringBuilderEntityData>
	{
		public new FrostySdk.Ebx.WSStringBuilderEntityData Data => data as FrostySdk.Ebx.WSStringBuilderEntityData;
		public override string DisplayName => "WSStringBuilder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSStringBuilderEntity(FrostySdk.Ebx.WSStringBuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

