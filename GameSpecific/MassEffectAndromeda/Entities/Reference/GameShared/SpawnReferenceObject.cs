using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnReferenceObjectData))]
	public class SpawnReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.SpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.SpawnReferenceObjectData Data => data as FrostySdk.Ebx.SpawnReferenceObjectData;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                outEvents.AddRange(base.Events);
                outEvents.Add(new ConnectionDesc("Spawn", Direction.In));
                outEvents.Add(new ConnectionDesc("Unspawn", Direction.In));
                outEvents.Add(new ConnectionDesc("OnSpawned", Direction.Out));
                return outEvents;
            }
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                outProperties.AddRange(base.Properties);
                outProperties.Add(new ConnectionDesc("Team", Direction.In));
                outProperties.Add(new ConnectionDesc("SetTeamOnSpawn", Direction.In));
                return outProperties;
            }
        }

        public SpawnReferenceObject(FrostySdk.Ebx.SpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

