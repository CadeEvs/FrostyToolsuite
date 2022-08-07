using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FrostySdk.Attributes;
using LevelEditorPlugin.Editors;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
    public interface ISchematicsType
    {
        EntityWorld World { get; }
        void BeginSimulation();
        void EndSimulation();

        void AddPropertyConnection(int srcPort, ISchematicsType dstObject, int dstPort);
        void AddEventConnection(int srcPort, ISchematicsType dstObject, int dstPort);
        void AddLinkConnection(int srcPort, ISchematicsType dstObject, int dstPort);

        void Update_PreFrame();
        void Update_PostFrame();

        IProperty GetProperty(int nameHash);
        IEvent GetEvent(int nameHash);
        ILink GetLink(int nameHash);

        void OnEvent(int eventHash);
        void OnPropertyChanged(int propertyHash);
        void OnLinkChanged(int linkHash);
    }

    public interface ILogicEntity : IEbxType, ISchematicsType
    {
        string DisplayName { get; }
        string Icon { get; }
        FrostySdk.Ebx.Realm Realm { get; }
        IEnumerable<ConnectionDesc> Links { get; }
        IEnumerable<ConnectionDesc> Events { get; }
        IEnumerable<ConnectionDesc> Properties { get; }
        IEnumerable<string> HeaderRows { get; }
        IEnumerable<string> DebugRows { get; }
        byte FlagsPropertyRealm { get; set; }
        byte FlagsEventRealm { get; set; }
        byte FlagsLinkRealm { get; set; }
    }

    public enum Direction
    {
        In,
        Out
    }

    public struct Any
    {
    }
    public struct InputEvent
    {
    }
    public struct OutputEvent
    {
    }

    public interface IProperty
    {
        int NameHash { get; }
        object Value { get; set; }
        bool HasValueChanged { get; }
        bool IsUnset { get; }
        long SetOnFrame { get; }
        void AddConnection(ISchematicsType from, int fromPort);
    }
    public interface ILink
    {
        int NameHash { get; }
        object Value { get; set; }
        bool HasValueChanged { get; }
        bool IsUnset { get; }
        void AddConnection(ISchematicsType from, int fromPort);
    }
    public interface IEvent
    {
        int NameHash { get; }
        ISchematicsType Owner { get; }
        long SetOnFrame { get; }
        bool IsBound { get; }
        void AddConnection(ISchematicsType from, int fromPort);
        void Execute();
    }

    public class Property<T> : IProperty
    {
        public int NameHash => nameHash;
        object IProperty.Value
        {
            get => Value;
            set => Value = (T)value;
        }
        public T Value
        {
            get
            {
                return GetValueOrDefault(currentValue);
            }
            set
            {
                if (Compare(currentValue, value))
                {
                    setOnFrame = owner.World.SimulationFrameCount;
                    currentValue = value;
                    isUnset = false;

                    if (outConnections.Count == 0)
                    {
                        // notify if this is a target
                        owner.OnPropertyChanged(nameHash);
                    }
                    else
                    {
                        // push if this is a source
                        foreach (var connection in outConnections)
                        {
                            connection.Value = value;
                        }
                    }
                }
            }
        }
        public bool HasValueChanged
        {
            get
            {
                return false;
            }
        }
        public bool IsUnset
        {
            get => isUnset;
        }
        public long SetOnFrame
        {
            get => setOnFrame;
        }

        private ISchematicsType owner;
        private T currentValue;
        private T originalValue;
        private int nameHash;
        private string debugName;
        private long setOnFrame;

        private List<IProperty> inConnections = new List<IProperty>();
        private List<IProperty> outConnections = new List<IProperty>();

        private bool isUnset;

        public Property(ISchematicsType inOwner, int inNameHash)
            : this(inOwner, inNameHash, default(T))
        {
        }

        public Property(ISchematicsType inOwner, int inNameHash, T inDefaultValue, string inOptionalName = "")
        {
            owner = inOwner;
            nameHash = inNameHash;
            debugName = string.IsNullOrEmpty(inOptionalName) ? inNameHash.ToString("x8") : inOptionalName;
            originalValue = inDefaultValue;
            currentValue = originalValue;
            isUnset = true;

            var properties = (List<IProperty>)Library.Reflection.ReflectionUtils.GetPrivateField(owner.GetType(), owner, "properties");
            properties.Add(this);
        }

        public void AddConnection(ISchematicsType from, int fromPort)
        {
            var property = from.GetProperty(fromPort);
            if (property != null)
            {
                var properties = (List<IProperty>)Library.Reflection.ReflectionUtils.GetPrivateField(property.GetType(), property, "outConnections");
                properties.Add(this);

                inConnections.Add(property);
            }
        }

        protected bool Compare(object a, object b)
        {
            var tmpA = a;
            var tmpB = b;

            //if (tmpA == null) tmpA = default(T);
            //if (tmpB == null) tmpB = default(T);

            if (isUnset)
                return true;

            if (tmpA == null && tmpB == null)
                return false;

            if (tmpA == null && tmpB != null)
                return true;

            if (tmpB == null && tmpA != null)
                return true;

            return !tmpA.Equals(tmpB);
        }

        protected T GetValueOrDefault(T inValue)
        {
            if (inValue == null)
            {
                var valueType = typeof(T);
                var attr = valueType.GetCustomAttribute<EbxClassMetaAttribute>();
                if (attr != null && attr.Type == FrostySdk.IO.EbxFieldType.Struct)
                {
                    inValue = (T)Activator.CreateInstance(valueType);
                }
            }
            return inValue;
        }

        public override string ToString()
        {
            return $"{debugName}: {typeof(T)} ({currentValue.ToString()})";
        }
    }

    public class Link<T> : ILink
    {
        public int NameHash => nameHash;
        object ILink.Value
        {
            get => Value;
            set
            {
                if (typeof(T).GetInterface("IList") != null)
                {
                    AddElement(value);
                }
                else
                {
                    Value = (T)value;
                }
            }
        }
        public T Value
        {
            get
            {
                if (!owner.World.IsSimulationRunning)
                {
                    return originalValue;
                }
                return currentValue;
            }
            set
            {
                if (typeof(T).GetInterface("IList") != null)
                {
                    AddElement(value);
                }
                else
                {
                    if (Compare(currentValue, value))
                    {
                        setOnFrame = owner.World.SimulationFrameCount;
                        currentValue = value;
                        isUnset = false;

                        if (outConnections.Count == 0)
                        {
                            // notify if this is a target
                            owner.OnLinkChanged(nameHash);
                        }
                        else
                        {
                            // push if this is a source
                            foreach (var connection in outConnections)
                            {
                                connection.Value = value;
                            }
                        }
                    }
                }
            }
        }
        public bool HasValueChanged
        {
            get
            {
                return false;
            }
        }
        public bool IsUnset
        {
            get => isUnset;
        }
        public long SetOnFrame
        {
            get => setOnFrame;
        }

        private ISchematicsType owner;
        private T currentValue;
        private T originalValue;
        private int nameHash;
        private string debugName;
        private long setOnFrame;

        private List<ILink> inConnections = new List<ILink>();
        private List<ILink> outConnections = new List<ILink>();

        private bool isUnset;

        public Link(ISchematicsType inOwner, int inNameHash)
            : this(inOwner, inNameHash, default(T))
        {
        }

        public Link(ISchematicsType inOwner, int inNameHash, T inDefaultValue, string inOptionalName = "")
        {
            if (typeof(T).GetInterface("IList") != null)
            {
                inDefaultValue = (T)Activator.CreateInstance(typeof(T));
            }

            owner = inOwner;
            nameHash = inNameHash;
            debugName = string.IsNullOrEmpty(inOptionalName) ? inNameHash.ToString("x8") : inOptionalName;
            originalValue = inDefaultValue;
            currentValue = originalValue;
            isUnset = true;

            var links = (List<ILink>)Library.Reflection.ReflectionUtils.GetPrivateField(owner.GetType(), owner, "links");
            links.Add(this);
        }

        public void AddConnection(ISchematicsType from, int fromPort)
        {
            var link = from.GetLink(fromPort);
            if (link != null)
            {
                var links = (List<ILink>)Library.Reflection.ReflectionUtils.GetPrivateField(link.GetType(), link, "outConnections");
                links.Add(this);

                inConnections.Add(link);
            }
        }

        protected bool Compare(object a, object b)
        {
            var tmpA = a;
            var tmpB = b;

            //if (tmpA == null) tmpA = default(T);
            //if (tmpB == null) tmpB = default(T);

            if (isUnset)
                return true;

            if (tmpA == null && tmpB == null)
                return false;

            if (tmpA == null && tmpB != null)
                return true;

            if (tmpB == null && tmpA != null)
                return true;

            return !tmpA.Equals(tmpB);
        }

        protected void AddElement(object element)
        {
            IList list = (IList)currentValue;
            list.Add(element);

            setOnFrame = owner.World.SimulationFrameCount;
            isUnset = false;

            if (outConnections.Count == 0)
            {
                owner.OnLinkChanged(nameHash);
            }
            else
            {
                // push if this is a source
                foreach (var connection in outConnections)
                {
                    connection.Value = element;
                }
            }
        }

        public override string ToString()
        {
            return $"{debugName}: {typeof(T)} ({currentValue.ToString()})";
        }
    }

    public class LinkArray<T> : Link<List<T>>
    {
        public LinkArray(ISchematicsType inOwner, int inNameHash)
            : base(inOwner, inNameHash)
        {
        }
    }

    public class Event<T> : IEvent
    {
        public int NameHash => nameHash;
        public ISchematicsType Owner => owner;
        public long SetOnFrame
        {
            get => setOnFrame;
        }
        public bool IsBound
        {
            get => connections.Count > 0;
        }

        private ISchematicsType owner;
        private int nameHash;
        private string debugName;
        private List<IEvent> connections = new List<IEvent>();
        private long setOnFrame;

        public Event(ISchematicsType inOwner, int inNameHash, string inOptionalName = "")
        {
            owner = inOwner;
            nameHash = inNameHash;
            debugName = string.IsNullOrEmpty(inOptionalName) ? inNameHash.ToString("x8") : inOptionalName;

            var events = (List<IEvent>)Library.Reflection.ReflectionUtils.GetPrivateField(owner.GetType(), owner, "events");
            events.Add(this);
        }

        public void AddConnection(ISchematicsType from, int fromPort)
        {
            var evt = from.GetEvent(fromPort);
            if (evt != null)
            {
                connections.Add(evt);
            }
        }

        public void Execute()
        {
            setOnFrame = owner.World.SimulationFrameCount;
            foreach (var evt in connections)
            {
                evt.Owner.OnEvent(evt.NameHash);
            }
        }
    }

    public struct ConnectionDesc
    {
        public string Name;
        public Direction Direction;
        public Type DataType;
        public bool IsTriggerable;

        public ConnectionDesc(string name, Direction direction)
        {
            Name = name;
            Direction = direction;
            DataType = typeof(Any);
            IsTriggerable = false;
        }

        public ConnectionDesc(string name, Direction direction, bool triggerable)
        {
            Name = name;
            Direction = direction;
            DataType = typeof(Any);
            IsTriggerable = triggerable;
        }

        public ConnectionDesc(string name, Direction direction, Type dataType)
        {
            Name = name;
            Direction = direction;
            DataType = dataType;
            IsTriggerable = false;
        }
    }

    public enum UpdatePass
    {
        PreFrame,
        PostFrame
    }

    public class LogicEntity : Entity, ILogicEntity
    {
        public FrostySdk.Ebx.GameObjectData Data => data as FrostySdk.Ebx.GameObjectData;
        public virtual string Icon => "";
        public virtual IEnumerable<ConnectionDesc> Links { get => new List<ConnectionDesc>(); }
        public virtual IEnumerable<ConnectionDesc> Events { get => new List<ConnectionDesc>(); }
        public virtual IEnumerable<ConnectionDesc> Properties { get => new List<ConnectionDesc>(); }
        public virtual IEnumerable<string> TriggerableEvents { get => new List<string>(); }
        public virtual IEnumerable<string> HeaderRows { get => new List<string>(); }
        public virtual IEnumerable<string> DebugRows { get => new List<string>(); }
        public int EventToTrigger
        {
            set
            {
                if (world.IsSimulationRunning)
                {
                    eventToTrigger = value;
                }
            }
        }
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
        protected int eventToTrigger;

        public LogicEntity(FrostySdk.Ebx.GameObjectData inData, Entity inParent)
            : base(inData, inParent)
        {
            flagsPropertyRealm = (byte)((Data.Flags >> 27) & 0x03);
            flagsEventRealm = (byte)((Data.Flags >> 25) & 0x03);
            flagsLinkRealm = (byte)((Data.Flags >> 25) & 0x33);
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
            if (eventToTrigger != 0)
            {
                var evt = GetEvent(eventToTrigger);
                if (evt != null)
                {
                    evt.Execute();
                }
                eventToTrigger = 0;
            }
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

        protected void CallEvent(int eventHash)
        {
            var evt = GetEvent(eventHash);
            if (evt == null)
                return;

            evt.Execute();
        }
    }
}
