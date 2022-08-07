using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Frosty.Core;
using System.IO;
using Frosty.Core.Managers;
using FrostySdk.Ebx;
using FrostySdk.IO;
using DataField = FrostySdk.Ebx.DataField;
using LevelEditorPlugin.Converters;
using Asset = LevelEditorPlugin.Assets.Asset;
using ObjectBlueprint = LevelEditorPlugin.Assets.ObjectBlueprint;
using PrefabBlueprint = LevelEditorPlugin.Assets.PrefabBlueprint;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.ReferenceObjectData))]
    public class ReferenceObject : Entity, ISpatialReferenceEntity, ILogicEntity
    {
        public FrostySdk.Ebx.ReferenceObjectData Data => data as FrostySdk.Ebx.ReferenceObjectData;
        public InterfaceDescriptor InterfaceDescriptor => interfaceDescriptor;
        public override string DisplayName => (blueprint != null) ? Path.GetFileName(blueprint.Name) : base.DisplayName;
        public long UniqueId => uniqueId;
        public override bool RequiresTransformUpdate
        {
            get => base.RequiresTransformUpdate;
            set
            {
                foreach (Entity entity in entities)
                {
                    entity.RequiresTransformUpdate = value;
                }
            }
        }
        public Assets.Blueprint Blueprint => blueprint;
        public virtual string Icon => "";
        public virtual IEnumerable<ConnectionDesc> Links
        {
            get
            {
                List<ConnectionDesc> outLinks = new List<ConnectionDesc>();
                if (Blueprint != null)
                {
                    InterfaceDescriptorData interfaceDesc = Blueprint.Data.Interface.GetObjectAs<FrostySdk.Ebx.InterfaceDescriptorData>();
                    if (interfaceDesc != null)
                    {
                        foreach (DynamicLink linkDesc in interfaceDesc.InputLinks)
                        {
                            outLinks.Add(new ConnectionDesc() { Name = linkDesc.Name, Direction = Direction.Out });
                        }
                        foreach (DynamicLink linkDesc in interfaceDesc.OutputLinks)
                        {
                            outLinks.Add(new ConnectionDesc() { Name = linkDesc.Name, Direction = Direction.In });
                        }
                    }

                    if (blueprint is Assets.ObjectBlueprint)
                    {
                        if (entities.Count > 0)
                        {
                            Entity rootEntity = entities[0];
                            outLinks.AddRange((rootEntity as ILogicEntity).Links);
                        }
                    }
                }
                return outLinks;
            }
        }
        public virtual IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                if (Blueprint != null)
                {
                    InterfaceDescriptorData interfaceDesc = Blueprint.Data.Interface.GetObjectAs<FrostySdk.Ebx.InterfaceDescriptorData>();
                    if (interfaceDesc != null)
                    {
                        foreach (DynamicEvent eventDesc in interfaceDesc.InputEvents)
                        {
                            outEvents.Add(new ConnectionDesc() { Name = eventDesc.Name, Direction = Direction.In });
                        }
                        foreach (DynamicEvent eventDesc in interfaceDesc.OutputEvents)
                        {
                            outEvents.Add(new ConnectionDesc() { Name = eventDesc.Name, Direction = Direction.Out });
                        }
                    }

                    if (blueprint is Assets.ObjectBlueprint)
                    {
                        if (entities.Count > 0)
                        {
                            Entity rootEntity = entities[0];
                            outEvents.AddRange((rootEntity as ILogicEntity).Events);
                        }
                    }
                }
                return outEvents;
            }
        }
        public virtual IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                if (Blueprint != null)
                {
                    InterfaceDescriptorData interfaceDesc = Blueprint.Data.Interface.GetObjectAs<FrostySdk.Ebx.InterfaceDescriptorData>();
                    if (interfaceDesc != null)
                    {
                        foreach (DataField fieldDesc in interfaceDesc.Fields)
                        {
                            string fieldName = fieldDesc.Name;
                            if (Utils.IsFieldProperty(fieldName))
                                continue;

                            if (fieldDesc.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_Source)
                            {
                                outProperties.Add(new ConnectionDesc() { Name = fieldDesc.Name, Direction = Direction.Out });
                            }
                            else if (fieldDesc.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_Target)
                            {
                                outProperties.Add(new ConnectionDesc() { Name = fieldDesc.Name, Direction = Direction.In });
                            }
                            else
                            {
                                outProperties.Add(new ConnectionDesc() { Name = fieldDesc.Name, Direction = Direction.In });
                                outProperties.Add(new ConnectionDesc() { Name = fieldDesc.Name, Direction = Direction.Out });
                            }
                        }
                    }

                    if (blueprint is Assets.ObjectBlueprint)
                    {
                        if (entities.Count > 0)
                        {
                            Entity rootEntity = entities[0];
                            outProperties.AddRange((rootEntity as ILogicEntity).Properties);
                        }
                    }
                }
                outProperties.Add(new ConnectionDesc("BlueprintTransform", Direction.In));
                return outProperties;
            }
        }
        public virtual IEnumerable<string> HeaderRows { get { return new List<string>(); } }
        public virtual IEnumerable<string> DebugRows { get { return new List<string>(); } }
        public byte FlagsPropertyRealm
        {
            get => flagsPropertyRealm;
            set
            {
                if (flagsPropertyRealm >= value)
                    return;

                flagsPropertyRealm = value;
                Data.Flags |= (uint)value << 27;
            }
        }
        public byte FlagsEventRealm
        {
            get => flagsEventRealm;
            set
            {
                if (flagsEventRealm >= value)
                    return;

                flagsEventRealm = value;
                Data.Flags |= (uint)value << 25;
            }
        }
        public byte FlagsLinkRealm
        {
            get => flagsLinkRealm;
            set
            {
                if (flagsLinkRealm >= value)
                    return;

                flagsLinkRealm = value;
                Data.Flags |= (uint)value << 25;
            }
        }

        protected long uniqueId;

        protected byte flagsPropertyRealm;
        protected byte flagsEventRealm;
        protected byte flagsLinkRealm;

        protected Assets.Blueprint blueprint;
        protected List<Entity> entities = new List<Entity>();
        protected List<Assets.Asset> additionalAssets = new List<Assets.Asset>();
        protected InterfaceDescriptor interfaceDescriptor;

        protected List<IProperty> properties = new List<IProperty>();
        protected List<IEvent> events = new List<IEvent>();
        protected List<ILink> links = new List<ILink>();

        public ReferenceObject(FrostySdk.Ebx.ReferenceObjectData inData, Entity inParent, EntityWorld inWorld)
            : base(inData, inParent)
        {
            if (inWorld != null)
            {
                world = inWorld;
                world.AddEntity(this);

                uniqueId = world.GenerateDeterministicId(this);
            }

            Initialize();
            if (blueprint != null)
            {
                if (blueprint.Data.Interface.Type != FrostySdk.IO.PointerRefType.Null)
                {
                    interfaceDescriptor = new InterfaceDescriptor(blueprint.Data.Interface.GetObjectAs<FrostySdk.Ebx.InterfaceDescriptorData>(), this);
                }
            }
            SpawnEntities();
            CollectPropertyValues();
            //InitializeSchematics();
        }

        public ReferenceObject(FrostySdk.Ebx.ReferenceObjectData inData, Entity inParent)
            : this(inData, inParent, null)
        {   
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            foreach (Entity entity in entities)
                entity.CreateRenderProxy(proxies, state);
        }

        public override void SetOwner(Entity newOwner)
        {
            base.SetOwner(newOwner);
            foreach (Entity entity in entities)
                entity.SetOwner(newOwner);
        }

        public override void SetVisibility(bool newVisibility)
        {
            if (newVisibility != isVisible)
            {
                isVisible = newVisibility;
                foreach (Entity entity in entities)
                    entity.SetVisibility(newVisibility);
            }
        }

        public override void Destroy()
        {
            interfaceDescriptor?.Destroy();

            LoadedAssetManager.Instance.UnloadAsset(blueprint);
            foreach (Entity entity in entities)
                entity.Destroy();

            foreach (Asset asset in additionalAssets)
                LoadedAssetManager.Instance.UnloadAsset(asset);

            base.Destroy();
        }

        public virtual Matrix GetTransform()
        {
            Matrix m = Matrix.Identity;
            if (parent != null && parent is ISpatialEntity)
                m = (parent as ISpatialEntity).GetTransform();
            return SharpDXUtils.FromLinearTransform(Data.BlueprintTransform) * m;
        }

        public virtual Matrix GetLocalTransform()
        {
            return SharpDXUtils.FromLinearTransform(Data.BlueprintTransform);
        }

        public virtual void SetTransform(Matrix m, bool suppressUpdate)
        {
            if (suppressUpdate)
            {
                if (!UndoManager.Instance.IsUndoing && UndoManager.Instance.PendingUndoUnit == null)
                {
                    UndoManager.Instance.PendingUndoUnit = new MoveEntityUndoUnit(this, GetLocalTransform());
                }
            }
            else
            {
                UndoManager.Instance.CommitUndo(UndoManager.Instance.PendingUndoUnit);
            }

            Data.BlueprintTransform = MakeLinearTransform(m);
            NotifyEntityModified("BlueprintTransform");
        }

        public Entity FindEntity(Guid instanceGuid)
        {
            foreach (Entity entity in entities)
            {
                Guid entityInstanceGuid = entity.InstanceGuid;
                if (entityInstanceGuid == instanceGuid)
                    return entity;

                if (entity is ILayerEntity)
                {
                    if (entity is ReferenceObject)
                    {
                        Entity foundEntity = (entity as ReferenceObject).FindEntity(instanceGuid);
                        if (foundEntity != null)
                            return foundEntity;
                    }
                }
                else if (entity is IContainerOfEntities)
                {
                    Entity foundEntity = (entity as IContainerOfEntities).FindEntity(instanceGuid);
                    if (foundEntity != null)
                        return foundEntity;
                }
            }

            return null;
        }

        public virtual void BeginSimulation()
        {
            interfaceDescriptor?.BeginSimulation();
            selfLink.Value = this;

            foreach (Entity entity in entities)
            {
                if (entity is ISchematicsType)
                {
                    ISchematicsType schematicsEntity = entity as ISchematicsType;
                    schematicsEntity.BeginSimulation();
                }
            }
        }

        public virtual void EndSimulation()
        {
            interfaceDescriptor?.EndSimulation();
            foreach (Entity entity in entities)
            {
                if (entity is ISchematicsType)
                {
                    ISchematicsType schematicsEntity = entity as ISchematicsType;
                    schematicsEntity.EndSimulation();
                }
            }
        }

        public virtual void Update_PreFrame()
        {
            if (interfaceDescriptor != null)
            {
                interfaceDescriptor.Update_PreFrame();
            }

            // update property values in preframe
            //foreach (var property in properties)
            //{
            //    if (property.HasValueChanged)
            //    {
            //        OnPropertyChanged(property.NameHash);
            //    }
            //}

            // update link values in preframe
            foreach (ILink link in links)
            {
                if (link.HasValueChanged)
                {
                    OnLinkChanged(link.NameHash);
                }
            }

            foreach (Entity entity in entities)
            {
                if (entity is ISchematicsType)
                {
                    ISchematicsType schematicsEntity = entity as ISchematicsType;
                    schematicsEntity.Update_PreFrame();
                }
            }
        }

        public virtual void Update_PostFrame()
        {
            if (interfaceDescriptor != null)
            {
                interfaceDescriptor.Update_PostFrame();
            }

            foreach (Entity entity in entities)
            {
                if (entity is ISchematicsType)
                {
                    ISchematicsType schematicsEntity = entity as ISchematicsType;
                    schematicsEntity.Update_PostFrame();
                }
            }
        }

        public virtual void AddPropertyConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            IProperty property = properties.Find(p => p.NameHash == srcPort);
            if (property != null)
            {
                property.AddConnection(dstObject, dstPort);
                return;
            }

            interfaceDescriptor?.AddPropertyConnection(srcPort, dstObject, dstPort);
        }

        public void AddEventConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            interfaceDescriptor?.AddEventConnection(srcPort, dstObject, dstPort);
        }

        public void AddLinkConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            interfaceDescriptor?.AddLinkConnection(srcPort, dstObject, dstPort);
        }

        public IProperty GetProperty(int nameHash)
        {
            IProperty property = interfaceDescriptor?.GetProperty(nameHash);
            if (property != null)
                return property;

            return properties.Find(p => p.NameHash == nameHash);
        }

        public IEvent GetEvent(int nameHash)
        {
            IEvent evt = interfaceDescriptor?.GetEvent(nameHash);
            if (evt != null)
                return evt;

            return events.Find(e => e.NameHash == nameHash);
        }

        public ILink GetLink(int nameHash)
        {
            ILink link = interfaceDescriptor?.GetLink(nameHash);
            if (link != null)
                return link;

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

        public void AddProperty(IProperty property)
        {
            properties.Add(property);
        }

        public virtual void AddEntity(Entity inEntity)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveEntity(Entity inEntity)
        {
            throw new NotImplementedException();
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Data.LightmapScaleWithSize = true;
#if MASS_EFFECT
            Data.OverrideSpawningRadius = -1;
#endif
        }

        protected virtual void Initialize()
        {
            blueprint = LoadedAssetManager.Instance.LoadAsset<Assets.Blueprint>(this, Data.Blueprint);
        }

        protected virtual void SpawnEntities()
        {
            if (blueprint == null /*|| (this is INotRealSpatialEntity && Parent != null)*/)
                return;

            if (blueprint is Assets.ObjectBlueprint)
            {
                ObjectBlueprint objectBlueprint = blueprint as Assets.ObjectBlueprint;
                GameObjectData objectData = objectBlueprint.Data.Object.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                Entity entity = CreateEntity(objectData);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            else if (blueprint is Assets.PrefabBlueprint)
            {
                PrefabBlueprint prefabBlueprint = blueprint as Assets.PrefabBlueprint;
                int index = 0;

                foreach (PointerRef objPointer in prefabBlueprint.Data.Objects)
                {
                    if (objPointer.Type == FrostySdk.IO.PointerRefType.External)
                    {
                        // make sure the external asset is already loaded and add a ref to it
                        additionalAssets.Add(LoadedAssetManager.Instance.LoadAsset<Assets.Asset>(objPointer.External.FileGuid));
                    }

                    GameObjectData objectData = objPointer.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                    Entity entity = CreateEntity(objectData);

                    if (entity != null)
                    {
                        entities.Add(entity);
                    }

                    if (parent is SubWorldReferenceObject)
                    {
                        // only update the task window with layers that are associated with the current level (not layers within spatial prefabs)
                        LevelEditor.UpdateTask($"{Path.GetFileName(Blueprint.Name)}", (index++ / (double)prefabBlueprint.Data.Objects.Count) * 100);
                    }
                }

                EbxAsset ebxAsset = LoadedAssetManager.Instance.GetEbxAsset(blueprint.FileGuid);
                foreach (object instance in ebxAsset.Objects)
                {
                    if (instance is FrostySdk.Ebx.BaseShapeData)
                    {
                        Entity entity = CreateEntity(instance as FrostySdk.Ebx.GameObjectData);
                        if (entity != null)
                        {
                            entities.Add(entity);
                        }
                    }
                }
            }
        }

        protected virtual void CollectPropertyValues()
        {
            //if (this is LayerReferenceObject || this is SubWorldReferenceObject)
            if (interfaceDescriptor != null)
            {
                // find all property values
                foreach (PropertyConnection propertyConnection in Blueprint.Data.PropertyConnections.Where(pc => Utils.IsFieldProperty(((string)pc.SourceField))))
                {
                    ReferenceObject layerEntity = entities.Where(le =>
                    {
                        ReferenceObject refObjEntity = le as ReferenceObject;
                        return refObjEntity?.Blueprint?.FileGuid == propertyConnection.Target.External.FileGuid;

                    }).FirstOrDefault() as ReferenceObject;

                    if (layerEntity == null)
                    {
                        layerEntity = this;
                    }

                    Entity entity = layerEntity?.FindEntity(propertyConnection.Target.GetInstanceGuid());
                    if (entity != null)
                    {
                        DataField field = interfaceDescriptor.Data.Fields.Find(f => f.Id == propertyConnection.SourceFieldId);
                        entity.AddPropertyValue(propertyConnection.TargetField, field, propertyConnection, Blueprint);
                    }
                }
            }
        }

        public void InitializeSchematics()
        {
            if (Blueprint == null)
                return;

            foreach (PropertyConnection propertyConnection in Blueprint.Data.PropertyConnections)
            {
                ISchematicsType sourceEntity = world.FindEntity(propertyConnection.Source.GetInstanceGuid()) as ISchematicsType;
                ISchematicsType targetEntity = world.FindEntity(propertyConnection.Target.GetInstanceGuid()) as ISchematicsType;

                if (sourceEntity == null) sourceEntity = interfaceDescriptor;
                if (targetEntity == null) targetEntity = interfaceDescriptor;

                if (targetEntity == null || sourceEntity == null)
                {
                    Debug.WriteLine($"Unable to add connection for {propertyConnection.Source.GetInstanceGuid()} -> {propertyConnection.Target.GetInstanceGuid()} in {Blueprint.Name}");
                    continue;
                }

                targetEntity.AddPropertyConnection(propertyConnection.TargetFieldId, sourceEntity, propertyConnection.SourceFieldId);
            }

            foreach (EventConnection eventConnection in Blueprint.Data.EventConnections)
            {
                ISchematicsType sourceEntity = world.FindEntity(eventConnection.Source.GetInstanceGuid()) as ISchematicsType;
                ISchematicsType targetEntity = world.FindEntity(eventConnection.Target.GetInstanceGuid()) as ISchematicsType;

                if (sourceEntity == null) sourceEntity = interfaceDescriptor;
                if (targetEntity == null) targetEntity = interfaceDescriptor;

                if (targetEntity == null || sourceEntity == null)
                {
                    Debug.WriteLine($"Unable to add connection for {eventConnection.Source.GetInstanceGuid()} -> {eventConnection.Target.GetInstanceGuid()} in {Blueprint.Name}");
                    continue;
                }

                sourceEntity.AddEventConnection(eventConnection.SourceEvent.Id, targetEntity, eventConnection.TargetEvent.Id);
            }

            foreach (LinkConnection linkConnection in Blueprint.Data.LinkConnections)
            {
                ISchematicsType sourceEntity = world.FindEntity(linkConnection.Source.GetInstanceGuid()) as ISchematicsType;
                ISchematicsType targetEntity = world.FindEntity(linkConnection.Target.GetInstanceGuid()) as ISchematicsType;

                if (sourceEntity == null) sourceEntity = interfaceDescriptor;
                if (targetEntity == null) targetEntity = interfaceDescriptor;

                if (targetEntity == null || sourceEntity == null)
                {
                    Debug.WriteLine($"Unable to add connection for {linkConnection.Source.GetInstanceGuid()} -> {linkConnection.Target.GetInstanceGuid()} in {Blueprint.Name}");
                    continue;
                }

                sourceEntity.AddLinkConnection(linkConnection.SourceFieldId, targetEntity, linkConnection.TargetFieldId);
            }
        }
    }
}
