using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticTextOutputEntityData))]
	public class StaticTextOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StaticTextOutputEntityData>
	{
		public new FrostySdk.Ebx.StaticTextOutputEntityData Data => data as FrostySdk.Ebx.StaticTextOutputEntityData;
		public override string DisplayName => "StaticTextOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StaticTextOutputEntity(FrostySdk.Ebx.StaticTextOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

