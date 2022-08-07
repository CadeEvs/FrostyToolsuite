using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrostySdk.Ebx;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.TimelineTrackData))]
    public class TimelineTrack : Entity, IEntityData<FrostySdk.Ebx.TimelineTrackData>, ITimelineTrackEntity, IContainerOfEntities
    {
        public FrostySdk.Ebx.TimelineTrackData Data => data as FrostySdk.Ebx.TimelineTrackData;
        public virtual string Icon => "";
        public TimelineEntity Timeline => owningTimeline;
        public IEnumerable<TimelineTrack> Tracks => tracks;
        public virtual IEnumerable<ConnectionDesc> Events => new List<ConnectionDesc>();
        public virtual IEnumerable<ConnectionDesc> Properties => new List<ConnectionDesc>();

        protected List<TimelineTrack> tracks = new List<TimelineTrack>();
        protected TimelineEntity owningTimeline;

        public TimelineTrack(FrostySdk.Ebx.TimelineTrackData inData, Entity inParent)
            : base(inData, inParent)
        {
            owningTimeline = FindAncestor<TimelineEntity>();
        }

        public void AddEntity(Entity inEntity)
        {
        }

        public void RemoveEntity(Entity inEntity)
        {
        }

        public override void Destroy()
        {
                base.Destroy();
        }

        public virtual void Initialize(ReferenceObject inLayer)
        {
        }

        public Entity FindEntity(Guid instanceGuid)
        {
            foreach (TimelineTrack track in tracks)
            {
                if (track.InstanceGuid == instanceGuid)
                    return track;
                
                Entity entity = track.FindEntity(instanceGuid);
                if (entity != null)
                    return entity;
            }

            return null;
        }

        public virtual void InitializeForSchematics(TimelineEntity owningTimeline)
        {
        }

        public virtual void Update(float elapsedTime)
        {
        }

        protected void AddTrack(FrostySdk.Ebx.PointerRef pr)
        {
            TimelineTrack trackEntity = CreateEntityFromRef(pr);
            if (trackEntity == null)
                return;
            tracks.Add(trackEntity);
        }

        protected void AddTrack<T>(FrostySdk.Ebx.PointerRef pr, out T outTrack) where T : TimelineTrack
        {
            TimelineTrack trackEntity = CreateEntityFromRef(pr);
            if (trackEntity == null)
            {
                outTrack = default(T);
                return;
            }

            tracks.Add(trackEntity);
            outTrack = trackEntity as T;
        }

        protected TimelineTrack CreateEntityFromRef(FrostySdk.Ebx.PointerRef pr)
        {
            TimelineTrackData objectData = pr.GetObjectAs<FrostySdk.Ebx.TimelineTrackData>();
            return CreateEntity(objectData) as TimelineTrack;
        }
    }
}
