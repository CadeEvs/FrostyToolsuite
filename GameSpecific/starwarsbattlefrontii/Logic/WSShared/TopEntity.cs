using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TopEntityData))]
	public class TopEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TopEntityData>
	{
		public new FrostySdk.Ebx.TopEntityData Data => data as FrostySdk.Ebx.TopEntityData;
		public override string DisplayName => "Top";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TopEntity(FrostySdk.Ebx.TopEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

