using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntHubEntityData))]
	public class IntHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntHubEntityData>
	{
		public new FrostySdk.Ebx.IntHubEntityData Data => data as FrostySdk.Ebx.IntHubEntityData;
		public override string DisplayName => "IntHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntHubEntity(FrostySdk.Ebx.IntHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

