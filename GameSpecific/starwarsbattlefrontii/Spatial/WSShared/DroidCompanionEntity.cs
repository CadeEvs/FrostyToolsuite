using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionEntityData))]
	public class DroidCompanionEntity : ActorEntity, IEntityData<FrostySdk.Ebx.DroidCompanionEntityData>
	{
		public new FrostySdk.Ebx.DroidCompanionEntityData Data => data as FrostySdk.Ebx.DroidCompanionEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DroidCompanionEntity(FrostySdk.Ebx.DroidCompanionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

