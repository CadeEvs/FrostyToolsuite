using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberSpawnReferenceObjectData))]
	public class PartyMemberSpawnReferenceObject : CharacterSpawnReferenceObject, IEntityData<FrostySdk.Ebx.PartyMemberSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.PartyMemberSpawnReferenceObjectData Data => data as FrostySdk.Ebx.PartyMemberSpawnReferenceObjectData;

		public PartyMemberSpawnReferenceObject(FrostySdk.Ebx.PartyMemberSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

