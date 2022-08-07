using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frosty.Core.Managers;
using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
    public class Component : Entity, IEntityData<FrostySdk.Ebx.ComponentData>, ISpatialEntity, ILogicEntity, IComponentEntity
    {
        public FrostySdk.Ebx.ComponentData Data => data as FrostySdk.Ebx.ComponentData;
        public override bool RequiresTransformUpdate
        {
            get => requiresTransformUpdate;
            set
            {
                requiresTransformUpdate = value;
                foreach (var entity in Components)
                    entity.RequiresTransformUpdate = value;
            }
        }
        public IEnumerable<Entity> Components => components;
        public virtual string Icon => "";
        public virtual IEnumerable<ConnectionDesc> Links { get { return new List<ConnectionDesc>(); } }
        public virtual IEnumerable<ConnectionDesc> Events { get { return new List<ConnectionDesc>(); } }
        public virtual IEnumerable<ConnectionDesc> Properties { get { return new List<ConnectionDesc>(); } }
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

        protected byte flagsPropertyRealm;
        protected byte flagsEventRealm;
        protected byte flagsLinkRealm;

        protected List<Entity> components = new List<Entity>();
        protected bool requiresTransformUpdate;

        protected List<IProperty> properties = new List<IProperty>();
        protected List<IEvent> events = new List<IEvent>();
        protected List<ILink> links = new List<ILink>();

        public Component(FrostySdk.Ebx.ComponentData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            foreach (var entity in Components)
            {
                entity.CreateRenderProxy(proxies, state);
            }
        }

        public override void SetOwner(Entity newOwner)
        {
            base.SetOwner(newOwner);
            foreach (var entity in Components)
                entity.SetOwner(newOwner);
        }

        public override void SetVisibility(bool newVisibility)
        {
            if (newVisibility != isVisible)
            {
                isVisible = newVisibility;
                foreach (var entity in Components)
                    entity.SetVisibility(newVisibility);
            }
        }

        public virtual Matrix GetTransform()
        {
            Matrix m = Matrix.Identity;
            if (parent != null && parent is ISpatialEntity)
                m = (parent as ISpatialEntity).GetTransform();
            return m;
        }

        public virtual Matrix GetLocalTransform()
        {
            return SharpDXUtils.FromLinearTransform(Data.Transform);
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

            NotifyEntityModified("");
        }

        public void SpawnComponents()
        {
            foreach (var objPointer in Data.Components)
            {
                if (objPointer.Type == FrostySdk.IO.PointerRefType.External)
                {
                    // make sure the external asset is already loaded and add a ref to it
                    //additionalAssets.Add(LoadedAssetManager.Instance.LoadAsset<Assets.Asset>(objPointer.External.FileGuid));
                    System.Diagnostics.Debug.Assert(false);
                }

                var objectData = objPointer.GetObjectAs<FrostySdk.Ebx.GameObjectData>();
                var component = CreateEntity(objectData);

                if (component != null)
                {
                    components.Add(component);
                    if (component is Component)
                    {
                        var gameComponent = component as Component;
                        gameComponent.SpawnComponents();
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

        public virtual void Update_PreFrame()
        {
        }

        public virtual void Update_PostFrame()
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

        public void AddEventConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            var evt = GetEvent(srcPort);
            if (evt == null)
            {
                evt = new Event<OutputEvent>(this, srcPort);
            }

            evt.AddConnection(dstObject, dstPort);
        }

        public void AddLinkConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            var link = GetLink(srcPort);
            if (link == null)
            {
                link = new Link<object>(this, srcPort);
            }

            link.AddConnection(dstObject, dstPort);
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

        public override void Destroy()
        {
            foreach (var component in components)
                component.Destroy();
        }
    }
}
