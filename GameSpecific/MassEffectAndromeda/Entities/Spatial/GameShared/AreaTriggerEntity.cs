using System.Collections.Generic;
using LinearTransform = FrostySdk.Ebx.LinearTransform;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AreaTriggerEntityData))]
	public class AreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.AreaTriggerEntityData>
	{
        protected readonly int Link_Geometry = Frosty.Hash.Fnv1.HashString("Geometry");
        protected readonly int Link_Soldiers = Frosty.Hash.Fnv1.HashString("Soldiers");
        protected readonly int Link_ObjectToFollow = Frosty.Hash.Fnv1.HashString("ObjectToFollow");

        public new FrostySdk.Ebx.AreaTriggerEntityData Data => data as FrostySdk.Ebx.AreaTriggerEntityData;
        public override IEnumerable<ConnectionDesc> Links
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Geometry", Direction.In, typeof(List<BaseShape>)),
                new ConnectionDesc("Soldiers", Direction.In, typeof(List<CharacterSpawnReferenceObject>)),
                new ConnectionDesc("ObjectToFollow", Direction.In, typeof(SpawnReferenceObject))
            };
        }
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Enable", Direction.In),
                new ConnectionDesc("Disable", Direction.In),
                new ConnectionDesc("OnEnabled", Direction.Out),
                new ConnectionDesc("OnDisabled", Direction.Out),
                new ConnectionDesc("OnEnter", Direction.Out),
                new ConnectionDesc("OnInsideArea", Direction.Out),
                new ConnectionDesc("OnLeave", Direction.Out)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Enabled", Direction.In, typeof(bool)),
                new ConnectionDesc("GeometryTransform", Direction.In, typeof(LinearTransform)),
                new ConnectionDesc("Radius", Direction.In, typeof(float)),
            };
        }

        protected LinkArray<BaseShape> geometryLink;
        protected LinkArray<CharacterSpawnReferenceObject> soldiersLink;
        protected Link<SpawnReferenceObject> objectToFollowLink;

        public AreaTriggerEntity(FrostySdk.Ebx.AreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            geometryLink = new LinkArray<BaseShape>(this, Link_Geometry);
            soldiersLink = new LinkArray<CharacterSpawnReferenceObject>(this, Link_Soldiers);
            objectToFollowLink = new Link<SpawnReferenceObject>(this, Link_ObjectToFollow);

        }
	}
}

