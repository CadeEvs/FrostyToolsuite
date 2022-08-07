using Frosty.Core.Viewport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.ComponentModel;
using System.Windows.Data;
using FrostySdk.Ebx;
using LevelEditorPlugin.Editors;

namespace LevelEditorPlugin.Entities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TypeProviderAttribute : Attribute
    {
        public string TypeName { get; set; }
        public Type Type { get; set; }

        public TypeProviderAttribute(string typeName, Type type)
        {
            TypeName = typeName;
            Type = type;
        }
    }
    public interface IObjectStringConverter
    {
        string ConvertToString(object value, bool includeType);
        object ConvertToObject(string value);
    }

    [TypeProvider("CString", typeof(FrostySdk.Ebx.CString))] public class CStringValueConverter : IObjectStringConverter { public object ConvertToObject(string value) { return (FrostySdk.Ebx.CString)value.Trim('"'); } public string ConvertToString(object value, bool includeType) { return $"{((includeType) ? "CString " : "")}\"{value}\""; } }
    [TypeProvider("Float32", typeof(float))] public class FloatValueConverter : IObjectStringConverter { public object ConvertToObject(string value) { return float.Parse(value); } public string ConvertToString(object value, bool includeType) { return $"{((includeType) ? "Float32 " : "")}{value}"; } }
    [TypeProvider("Int32", typeof(int))] public class IntValueConverter : IObjectStringConverter { public object ConvertToObject(string value) { return int.Parse(value); } public string ConvertToString(object value, bool includeType) { return $"{((includeType) ? "Int32 " : "")}{value}"; } }
    [TypeProvider("Boolean", typeof(bool))] public class BooleanValueConverter : IObjectStringConverter { public object ConvertToObject(string value) { return (value.Equals("True", StringComparison.OrdinalIgnoreCase) ? true : false); } public string ConvertToString(object value, bool includeType) { return $"{((includeType) ? "Boolean " : "")}{((bool)value ? "True" : "False")}"; } }
    [TypeProvider("Vec2", typeof(FrostySdk.Ebx.Vec2))]
    public class Vec2ValueConverter : IObjectStringConverter
    {
        public object ConvertToObject(string value)
        {
            string[] values = value.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new FrostySdk.Ebx.Vec2()
            {
                x = float.Parse(values[0]),
                y = float.Parse(values[1])
            };
        }
        public string ConvertToString(object value, bool includeType)
        {
            Vec2 vec = value as FrostySdk.Ebx.Vec2;
            return $"{((includeType) ? "Vec2 " : "")}({vec.x},{vec.y})";
        }
    }
    [TypeProvider("Vec3", typeof(FrostySdk.Ebx.Vec3))]
    public class Vec3ValueConverter : IObjectStringConverter
    {
        public object ConvertToObject(string value)
        {
            string[] values = value.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new FrostySdk.Ebx.Vec3()
            {
                x = float.Parse(values[0]),
                y = float.Parse(values[1]),
                z = float.Parse(values[2])
            };
        }
        public string ConvertToString(object value, bool includeType)
        {
            Vec3 vec = value as FrostySdk.Ebx.Vec3;
            return $"{((includeType) ? "Vec3 " : "")}({vec.x},{vec.y},{vec.z})";
        }
    }
    [TypeProvider("LinearTransform", typeof(FrostySdk.Ebx.LinearTransform))]
    public class LinearTransformValueConverter : IObjectStringConverter
    {
        private Vec3ValueConverter vec3ValueConverter = new Vec3ValueConverter();

        public object ConvertToObject(string value)
        {
            string[] values = value.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new FrostySdk.Ebx.LinearTransform()
            {
                right = new FrostySdk.Ebx.Vec3() { x = float.Parse(values[0]), y = float.Parse(values[1]), z = float.Parse(values[2]) },
                up = new FrostySdk.Ebx.Vec3() { x = float.Parse(values[3]), y = float.Parse(values[4]), z = float.Parse(values[5]) },
                forward = new FrostySdk.Ebx.Vec3() { x = float.Parse(values[6]), y = float.Parse(values[7]), z = float.Parse(values[8]) },
                trans = new FrostySdk.Ebx.Vec3() { x = float.Parse(values[9]), y = float.Parse(values[10]), z = float.Parse(values[11]) }
            };
        }
        public string ConvertToString(object value, bool includeType)
        {
            LinearTransform lt = value as FrostySdk.Ebx.LinearTransform;
            return $"{((includeType) ? "LinearTransform " : "")}({vec3ValueConverter.ConvertToString(lt.right, false)},{vec3ValueConverter.ConvertToString(lt.up, false)},{vec3ValueConverter.ConvertToString(lt.forward, false)},{vec3ValueConverter.ConvertToString(lt.trans, false)})";
        }
    }

    public class PropertyValue
    {
        public FrostySdk.Ebx.CString Name { get; set; } = "";
        public object Value { get; set; } = null;

        private IObjectStringConverter valueConverter;

        private FrostySdk.Ebx.DataField dataField;
        private FrostySdk.Ebx.PropertyConnection propConnection;
        private Assets.Blueprint blueprint;
        private bool usesOriginalData;

        public PropertyValue(FrostySdk.Ebx.CString name, FrostySdk.Ebx.DataField inDataField, FrostySdk.Ebx.PropertyConnection inPropConnection, Assets.Blueprint inBlueprint)
        {
            dataField = inDataField;
            propConnection = inPropConnection;
            blueprint = inBlueprint;
            usesOriginalData = !((string)inDataField.Name).StartsWith("PropertyValue_");

            string strValue = dataField.Value;
            string[] arr = strValue.Split(new[] { ' ' }, 2);

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterface("IObjectStringConverter") != null))
            {
                TypeProviderAttribute typeProvider = type.GetCustomAttribute<TypeProviderAttribute>();
                if (typeProvider == null)
                    continue;

                if (typeProvider.TypeName.Equals(arr[0], StringComparison.OrdinalIgnoreCase))
                {
                    valueConverter = (IObjectStringConverter)Activator.CreateInstance(type);
                    break;
                }
            }

            Name = name;
            if (valueConverter != null) Value = valueConverter.ConvertToObject(arr[1]);
            else Value = arr[1];
        }

        public void SetValue(object value)
        {
            Value = value;
            if (valueConverter != null)
            {
                if (usesOriginalData)
                {
                    // Need to create a new data field as other objects may be sharing this particular
                    // data field value.

                    string newName = blueprint.GetNextAvailablePropertyValue();
                    dataField = new FrostySdk.Ebx.DataField()
                    {
                        AccessType = dataField.AccessType,
                        Name = newName,
                        Value = "",
                        ValueRef = dataField.ValueRef
                    };

                    InterfaceDescriptorData interfaceDesc = blueprint.Data.Interface.GetObjectAs<FrostySdk.Ebx.InterfaceDescriptorData>();
                    interfaceDesc.Fields.Add(dataField);

                    // Replace the source field with our new source field
                    propConnection.SourceField = newName;
                    usesOriginalData = false;
                }

                dataField.Value = valueConverter.ConvertToString(value, true);
            }
        }
    }

    public interface IEbxType
    { 
        Guid FileGuid { get; }
        Guid InstanceGuid { get; }
    }

    public class EntityModifiedEventArgs : EventArgs
    {
        /// <summary>
        /// Tells the property grid which value is being updated. If this value is null then the
        /// entire object will be re-evaluated by the property grid
        /// </summary>
        public string OptionalParameterNameChanged { get; private set; }

        public EntityModifiedEventArgs(string inOptParamNameChanged)
        {
            OptionalParameterNameChanged = inOptParamNameChanged;
        }
    }

    [Flags]
    public enum EntityFlags
    {
        None = 0,
        Initialized = 1,
        RenderProxyGenerated = 2,
        Destroyed = 4,

        HasLogic = 0x1000
    }

    public abstract class Entity : INotifyPropertyChanged, IEbxType
    {
        public Entity Parent => parent;
        public Entity Owner => owner;
        public EntityWorld World => world;
        public object UserData
        {
            get => userData;
            set
            {
                if (userData != value)
                {
                    userData = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    SetVisibility(value);
                    NotifyPropertyChanged();
                }
            }
        }
        public virtual string DisplayName
        {
            get
            {
                return data.GetType().Name;
            }
        }
        public virtual FrostySdk.Ebx.Realm Realm => FrostySdk.Ebx.Realm.Realm_ClientAndServer;
        public Layers.SceneLayer Layer { get; set; }
        public virtual bool RequiresTransformUpdate
        {
            get => false;
            set
            {
            }
        }
        public Guid FileGuid => fileGuid;
        public Guid InstanceGuid => instanceGuid;

        protected EntityWorld world;
        protected Entity parent;
        protected Entity owner;

        protected FrostySdk.Ebx.GameObjectData data;
        protected bool isVisible;

        protected List<PropertyValue> propertyValues = new List<PropertyValue>();
        protected object mockDataObject;

        protected Guid fileGuid;
        protected Guid instanceGuid;

        protected Link<Entity> selfLink;
        protected object userData;

        protected EntityFlags flags;

        public event EventHandler<EntityModifiedEventArgs> EntityModified;

        public Entity(FrostySdk.Ebx.GameObjectData inData, Entity inParent)
        {
            data = inData;
            parent = inParent;
            owner = this;
            isVisible = true;

            if (inParent != null)
            {
                if (inParent is ReferenceObject)
                    fileGuid = (inParent as ReferenceObject).Blueprint.FileGuid;
                else if (inParent is LogicPrefabReferenceObject)
                    fileGuid = (inParent as LogicPrefabReferenceObject).Blueprint.FileGuid;
                else
                    fileGuid = inParent.FileGuid;

                world = inParent.world;
                world.AddEntity(this);
            }
            instanceGuid = Guid.Parse(data.__InstanceGuid.ToString());

            if (this is ISchematicsType)
            {
                // only entities that participate in schematics need the self link
                selfLink = new Link<Entity>(this as ISchematicsType, 0, this);
            }
        }

        public bool HasFlags(EntityFlags inFlags)
        {
            return (flags & inFlags) != 0;
        }

        public virtual void CreateRenderProxy(List<Render.Proxies.RenderProxy> proxies, RenderCreateState state)
        {
        }

        public virtual void SetParent(Entity newParent)
        {
            if (parent != newParent)
            {
                parent = newParent;
                fileGuid = parent.FileGuid;
            }
        }

        public virtual void SetOwner(Entity newOwner)
        {
            owner = newOwner;
        }

        public virtual void SetVisibility(bool newVisibility)
        {
            if (newVisibility != isVisible)
            {
                isVisible = newVisibility;
            }
        }

        public virtual void Destroy()
        {
            world.RemoveEntity(this);
            world = null;
        }

        // This function will return the appropriate data object to be displayed in the property grid
        // this might be different to the Data object of an entity.
        public virtual object GetPropertyGridData()
        {
            return data;
        }

        public void AddPropertyValue(string name, FrostySdk.Ebx.DataField dataField, FrostySdk.Ebx.PropertyConnection propConnection, Assets.Blueprint blueprint)
        {
            propertyValues.Add(new PropertyValue(name, dataField, propConnection, blueprint));
        }

        public List<PropertyValue> GetPropertyValues()
        {
            return propertyValues;
        }

        public object GetMockDataObject()
        {
            return mockDataObject;
        }

        public T FindAncestor<T>() where T : Entity
        {
            Entity p = parent;
            while (!(p is T))
            {
                p = p.parent;
                if (p == null)
                    return default(T);
            }
            return p as T;
        }

        public virtual void OnDataModified()
        {
        }

        public object GetRawData()
        {
            return data;
        }

        public virtual void SetDefaultValues()
        {
        }

        protected Entity CreateEntity(FrostySdk.Ebx.GameObjectData objectData)
        {
            return CreateEntity(objectData, this);
        }

        private static Dictionary<Type, Type> entityTypes = new Dictionary<Type, Type>();
        public static Entity CreateEntity(FrostySdk.Ebx.GameObjectData objectData, Entity parent, EntityWorld inWorld = null)
        {
            if (objectData == null)
                return null;

            if (entityTypes.Count == 0)
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<EntityBindingAttribute>() != null))
                {
                    EntityBindingAttribute attr = type.GetCustomAttribute<EntityBindingAttribute>();
                    entityTypes.Add(attr.DataType, type);
                }
            }

            Type objectDataType = objectData.GetType();
            if (entityTypes.ContainsKey(objectDataType))
            {
                Type objectType = entityTypes[objectDataType];
                return (inWorld != null)
                    ? (Entity)Activator.CreateInstance(objectType, new object[] { objectData, parent, inWorld })
                    : (Entity)Activator.CreateInstance(objectType, new object[] { objectData, parent });
            }

            System.Diagnostics.Debug.WriteLine(objectDataType.ToString());

            //if (objectDataType.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialEntityData)))
            //    System.Diagnostics.Debug.WriteLine(objectDataType.ToString());
            //else if (objectDataType.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialReferenceObjectData)))
            //    System.Diagnostics.Debug.WriteLine(objectDataType.ToString());

            return null;
        }
        public static Entity CreateEntity(FrostySdk.Ebx.GameObjectData objectData, Guid blueprintGuid, EntityWorld inWorld)
        {
            Entity entity = CreateEntity(objectData, null, inWorld);
            entity.fileGuid = blueprintGuid;
            return entity;
        }

        public static FrostySdk.Ebx.LinearTransform MakeLinearTransform(Matrix m)
        {
            FrostySdk.Ebx.LinearTransform lt = new FrostySdk.Ebx.LinearTransform();
            lt.right = new FrostySdk.Ebx.Vec3() { x = m.M11, y = m.M12, z = m.M13 };
            lt.up = new FrostySdk.Ebx.Vec3() { x = m.M21, y = m.M22, z = m.M23 };
            lt.forward = new FrostySdk.Ebx.Vec3() { x = m.M31, y = m.M32, z = m.M33 };
            lt.trans = new FrostySdk.Ebx.Vec3() { x = m.M41, y = m.M42, z = m.M43 };
            return lt;
        }

        public void NotifyEntityModified(string optParamNameChanged = null)
        {
            EntityModified?.Invoke(this, new EntityModifiedEventArgs(optParamNameChanged));
        }

        protected void SetFlags(EntityFlags inFlags)
        {
            flags |= inFlags;
        }

        protected void ClearFlags(EntityFlags inFlags)
        {
            flags ^= inFlags;
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public interface IEntityData<T>
    {
        T Data { get; }
    }

    /// <summary>
    /// Provides the entity with functionality for setting and getting a transform
    /// </summary>
    public interface ITransformEntity
    {
        Matrix GetTransform();
        Matrix GetLocalTransform();
        void SetTransform(Matrix m, bool suppressUpdate);
        bool RequiresTransformUpdate { get; set; }
    }

    /// <summary>
    /// Tells various systems that this entity has a spatial representation
    /// </summary>
    public interface ISpatialEntity : ITransformEntity
    {
    }
    public interface ISpatialReferenceEntity : ISpatialEntity
    {
    }

    /// <summary>
    /// Use on spatial entities to have them not show in any spatial lists. (eg. LogicPrefabs)
    /// </summary>
    public interface INotRealSpatialEntity
    {
    }

    /// <summary>
    /// Use on entities that can be removed from schematics view if it has zero connections, only use
    /// on entities where you will have a secondary list of instances to allow you to drag a new version
    /// into the canvas if required.
    /// </summary>
    public interface IHideInSchematicsView
    {
    }

    /// <summary>
    /// Entity is a layer and can hold other entities, and will appear in any layer based lists.
    /// </summary>
    public interface ILayerEntity
    {
        Layers.SceneLayer GetLayer();
    }

    /// <summary>
    /// Entity is a component or a holder of components, and will appear in any component based lists.
    /// </summary>
    public interface IComponentEntity
    {
        IEnumerable<Entity> Components { get; }
        void SpawnComponents();
    }

    /// <summary>
    /// Entity is a timeline track, and will appear in any timeline based lists
    /// </summary>
    public interface ITimelineTrackEntity
    {
        string Icon { get; }
        IEnumerable<TimelineTrack> Tracks { get; }
    }

    /// <summary>
    /// This tells the timeline editor to use this DisplayName instead of the default.
    /// </summary>
    public interface ITimelineCustomTrackName
    {
        string DisplayName { get; }
    }

    public interface IContainerOfEntities
    {
        Entity FindEntity(Guid instanceGuid);
        void AddEntity(Entity inEntity);
        void RemoveEntity(Entity inEntity);
    }

    public interface IAllowComponentsInLevel
    {
    }

    public class EntityBindingAttribute : Attribute
    {
        public Type DataType { get; set; }
    }
}
