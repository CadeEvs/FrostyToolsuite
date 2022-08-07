using LevelEditorPlugin.Editors;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntityTrackBaseData))]
	public class EntityTrackBase : TimelineTrack, IEntityData<FrostySdk.Ebx.EntityTrackBaseData>, ITimelineCustomTrackName, ITimelineEntityProviderTrack, ISchematicsType
	{
		protected readonly int Link_EntityLink = Frosty.Hash.Fnv1.HashString("EntityLink");

		public new FrostySdk.Ebx.EntityTrackBaseData Data => data as FrostySdk.Ebx.EntityTrackBaseData;
		public Entity Entity => entityLink.Value;
		string ITimelineCustomTrackName.DisplayName => trackName;
		public override string Icon => "Images/Tracks/EntityTrack.png";
		public override IEnumerable<ConnectionDesc> Events
		{
			get
			{
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.AddRange(base.Events);
				foreach (var track in tracks)
				{
					outEvents.AddRange(track.Events);
				}
				return outEvents;
			}
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				foreach (var track in tracks)
                {
					outProperties.AddRange(track.Properties);
                }
				return outProperties;
			}
		}
		public Entity EntityLink => entityLink.Value;

		protected string trackName;
		//protected Entity linkedEntity;

		protected List<IProperty> properties = new List<IProperty>();
		protected List<IEvent> events = new List<IEvent>();
		protected List<ILink> links = new List<ILink>();

		protected Link<Entity> entityLink;

		public EntityTrackBase(FrostySdk.Ebx.EntityTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
			foreach (var objPointer in Data.Children)
			{
				var objectData = objPointer.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
				var track = CreateEntity(objectData);

				if (track != null)
				{
					tracks.Add(track as TimelineTrack);
				}
			}

			entityLink = new Link<Entity>(this, Link_EntityLink);
		}

        public override void Initialize(ReferenceObject layerEntity)
        {
			base.Initialize(layerEntity);

			if (entityLink.Value != null)
			{
				var entity = entityLink.Value;
				trackName = (entity is ITimelineCustomTrackName) ? (entity as ITimelineCustomTrackName).DisplayName : entity.DisplayName;
			}

			//var validLink = layerEntity.Blueprint.Data.LinkConnections.Where(l => l.Source.GetObjectAs<FrostySdk.Ebx.GameObjectData>() == Data).FirstOrDefault();
			//if (validLink != null)
			//{
			//	trackName = validLink.Target.GetInstanceGuid().ToString();
			//	var target = layerEntity.FindEntity(validLink.Target.GetInstanceGuid());
			//	if (target != null)
			//	{
			//		linkedEntity = target;
			//		trackName = (target is ITimelineCustomTrackName) ? (target as ITimelineCustomTrackName).DisplayName : target.DisplayName;
			//	}
			//}
		}

        public override void Update(float elapsedTime)
        {
			if (entityLink.Value == null)
				return;

			foreach (var track in tracks)
            {
				track.Update(elapsedTime);
				if (track is LayeredTransformTrack)
				{
					var transformTrack = track as LayeredTransformTrack;

					var spatialEntity = entityLink.Value as ITransformEntity;
					if (spatialEntity != null)
					{
						Matrix currentTransform = (Matrix)transformTrack.CurrentValue;
						Matrix origTransform = spatialEntity.GetLocalTransform();

						Vector3 origTranslation;
						Vector3 origScale;
						Quaternion origRotation;

						origTransform.Decompose(out origScale, out origRotation, out origTranslation);

						spatialEntity.SetTransform(Matrix.Scaling(origScale) * currentTransform, true);
						spatialEntity.RequiresTransformUpdate = true;
					}
				}
				else if (track is PropertyTrackBase)
                {
					var propertyTrack = track as PropertyTrackBase;
					ISchematicsType schematicEntity = entityLink.Value as ISchematicsType;
					if (schematicEntity != null)
					{
						var property = schematicEntity.GetProperty(propertyTrack.Data.TargetPinId);
						if (property != null)
						{
							property.Value = propertyTrack.CurrentValue;
						}
					}
                }
            }
        }

		public virtual void BeginSimulation()
		{
			selfLink.Value = this;
		}

		public virtual void EndSimulation()
		{
		}

		public virtual void AddPropertyConnection(int srcPort, ISchematicsType dstObject, int dstPort)
		{
			var property = GetProperty(srcPort);
			if (property == null)
			{
				property = new Property<object>(this, srcPort);
			}
			property.AddConnection(dstObject, dstPort);
		}

		public virtual void AddEventConnection(int srcPort, ISchematicsType dstObject, int dstPort)
		{
			var evt = GetEvent(srcPort);
			if (evt == null)
			{
				evt = new Event<OutputEvent>(this, srcPort);
			}
			evt.AddConnection(dstObject, dstPort);
		}

		public virtual void AddLinkConnection(int srcPort, ISchematicsType dstObject, int dstPort)
		{
			var link = GetLink(srcPort);
			if (link == null)
			{
				link = new Link<object>(this, srcPort);
			}
			link.AddConnection(dstObject, dstPort);
		}

		public virtual void Update_PreFrame()
		{
		}

		public virtual void Update_PostFrame()
		{
		}

		public IProperty GetProperty(int nameHash)
		{
			return properties.Find(p => p.NameHash == nameHash);
		}

		public IEvent GetEvent(int nameHash)
		{
			return events.Find(e => e.NameHash == nameHash);
		}

		public ILink GetLink(int nameHash)
		{
			return links.Find(l => l.NameHash == nameHash);
		}

		public virtual void OnEvent(int eventHash)
		{
		}

		public virtual void OnPropertyChanged(int propertyHash)
		{
		}

		public virtual void OnLinkChanged(int linkHash)
		{
		}
	}
}

