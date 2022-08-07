using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frosty.Core.Managers;
using LevelEditorPlugin.Managers;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
    public class BaseShape : Entity, IEntityData<FrostySdk.Ebx.BaseShapeData>, ISpatialEntity, ILogicEntity
    {
        public FrostySdk.Ebx.BaseShapeData Data => data as FrostySdk.Ebx.BaseShapeData;
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

        protected List<IProperty> properties = new List<IProperty>();
        protected List<IEvent> events = new List<IEvent>();
        protected List<ILink> links = new List<ILink>();

        public BaseShape(FrostySdk.Ebx.BaseShapeData inData, Entity inParent)
            : base(inData, inParent)
        {
            flagsPropertyRealm = (byte)((Data.Flags >> 27) & 0x03);
            flagsEventRealm = (byte)((Data.Flags >> 25) & 0x03);
            flagsLinkRealm = (byte)((Data.Flags >> 25) & 0x33);
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
            return Matrix.Identity;
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
            IProperty property = GetProperty(srcPort);
            if (property == null)
            {
                property = new Property<object>(this, srcPort);
            }

            property.AddConnection(dstObject, dstPort);
        }

        public void AddEventConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            IEvent evt = GetEvent(srcPort);
            if (evt == null)
            {
                evt = new Event<OutputEvent>(this, srcPort);
            }

            evt.AddConnection(dstObject, dstPort);
        }

        public void AddLinkConnection(int srcPort, ISchematicsType dstObject, int dstPort)
        {
            ILink link = GetLink(srcPort);
            if (link == null)
            {
                link = new Link<object>(this, srcPort);
            }

            link.AddConnection(dstObject, dstPort);
        }

        public virtual IProperty GetProperty(int nameHash)
        {
            return properties.Find(p => p.NameHash == nameHash);
        }

        public virtual IEvent GetEvent(int nameHash)
        {
            return events.Find(e => e.NameHash == nameHash);
        }

        public virtual ILink GetLink(int nameHash)
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
                base.Destroy();
        }
    }
}
