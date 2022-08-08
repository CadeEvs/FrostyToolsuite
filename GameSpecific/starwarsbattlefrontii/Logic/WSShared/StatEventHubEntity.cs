using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StatEventHubEntityData))]
	public class StatEventHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StatEventHubEntityData>
	{
		public new FrostySdk.Ebx.StatEventHubEntityData Data => data as FrostySdk.Ebx.StatEventHubEntityData;
		public override string DisplayName => "StatEventHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StatEventHubEntity(FrostySdk.Ebx.StatEventHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

