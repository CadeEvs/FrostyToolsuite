using Frosty.Core;
using LevelEditorPlugin.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Extensions
{
    public class EntityGeneratorExtension : MenuExtension
    {
        public override string TopLevelMenuName => "Developer";
        public override string MenuItemName => "Generate Entity Boilerplate Code";
        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            string outPath = "Source\\Classes\\" + FrostySdk.ProfilesLibrary.ProfileName;

            var types = FrostySdk.TypeLibrary.GetConcreteTypes().Where(t => t.IsSubclassOf(typeof(FrostySdk.Ebx.EntityData))).ToList();
            while (types.Count > 0)
            {
                var type = types[0];
                RecursivelyCreateEntityBoilerplate(type, types, outPath);
            }

            types = FrostySdk.TypeLibrary.GetConcreteTypes().Where(t => t.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialEntityData))).ToList();
            while (types.Count > 0)
            {
                var type = types[0];
                RecursivelyCreateSpatialEntityBoilerplate(type, types, outPath);
            }

            types = FrostySdk.TypeLibrary.GetConcreteTypes().Where(t => t.IsSubclassOf(typeof(FrostySdk.Ebx.ReferenceObjectData))).ToList();
            while (types.Count > 0)
            {
                var type = types[0];
                RecursivelyCreateSpatialReferenceEntityBoilerplate(type, types, outPath);
            }

            types = FrostySdk.TypeLibrary.GetConcreteTypes().Where(t => t.IsSubclassOf(typeof(FrostySdk.Ebx.BaseShapeData))).ToList();
            while (types.Count > 0)
            {
                var type = types[0];
                RecursivelyCreateShapeBoilerplate(type, types, outPath);
            }

            types = FrostySdk.TypeLibrary.GetConcreteTypes().Where(t => t.IsSubclassOf(typeof(FrostySdk.Ebx.ComponentData))).ToList();
            while (types.Count > 0)
            {
                var type = types[0];
                RecursivelyCreateComponentBoilerplate(type, types, outPath);
            }

            types = FrostySdk.TypeLibrary.GetConcreteTypes().Where(t => t.IsSubclassOf(typeof(FrostySdk.Ebx.TimelineTrackData))).ToList();
            while (types.Count > 0)
            {
                var type = types[0];
                RecursivelyCreateTimelineTrackBoilerplate(type, types, outPath);
            }

            App.NotificationManager.Show("Generation complete");
            App.Logger.Log($"Generated entity boilerplate code to {outPath}");
        });

        private void RecursivelyCreateEntityBoilerplate(Type type, List<Type> types, string outPath)
        {
            string entityName = CreateEntityName(type);
            if (type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialEntityData)) || type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialReferenceObjectData)))
            {
                Type tmpType = type;
                while (tmpType != typeof(FrostySdk.Ebx.EntityData))
                {
                    types.Remove(tmpType);
                    tmpType = tmpType.BaseType;
                }

                return;
            }

            if (type.BaseType != typeof(FrostySdk.Ebx.EntityData))
            {
                RecursivelyCreateEntityBoilerplate(type.BaseType, types, outPath);
            }

            if (Type.GetType($"LevelEditorPlugin.Entities.{entityName}") != null)
            {
                types.Remove(type);
                return;
            }

            string code = CreateEntityBoilerplate(type);
            var attr = type.GetCustomAttribute<FrostySdk.Attributes.EbxClassMetaAttribute>();
            string newPath = outPath + $"/Logic/{attr.Namespace}/{entityName}.cs";

            FileInfo fi = new FileInfo(newPath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            using (TextWriter writer = new StreamWriter(new FileStream(newPath, FileMode.Create)))
            {
                writer.WriteLine(code);
            }

            types.Remove(type);
        }

        private void RecursivelyCreateSpatialEntityBoilerplate(Type type, List<Type> types, string outPath)
        {
            string entityName = CreateEntityName(type);
            if (type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialReferenceObjectData)))
            {
                Type tmpType = type;
                while (tmpType != typeof(FrostySdk.Ebx.SpatialEntityData))
                {
                    types.Remove(tmpType);
                    tmpType = tmpType.BaseType;
                }

                return;
            }

            if (type.BaseType != typeof(FrostySdk.Ebx.SpatialEntityData))
            {
                RecursivelyCreateSpatialEntityBoilerplate(type.BaseType, types, outPath);
            }

            if (Type.GetType($"LevelEditorPlugin.Entities.{entityName}") != null)
            {
                types.Remove(type);
                return;
            }

            string code = CreateSpatialEntityBoilerplate(type);
            var attr = type.GetCustomAttribute<FrostySdk.Attributes.EbxClassMetaAttribute>();
            string newPath = outPath + $"/Spatial/{attr.Namespace}/{entityName}.cs";

            FileInfo fi = new FileInfo(newPath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            using (TextWriter writer = new StreamWriter(new FileStream(newPath, FileMode.Create)))
            {
                writer.WriteLine(code);
            }

            types.Remove(type);
        }

        private void RecursivelyCreateSpatialReferenceEntityBoilerplate(Type type, List<Type> types, string outPath)
        {
            string entityName = CreateEntityName(type);
            if (type.BaseType != typeof(FrostySdk.Ebx.ReferenceObjectData))
            {
                RecursivelyCreateSpatialReferenceEntityBoilerplate(type.BaseType, types, outPath);
            }

            if (Type.GetType($"LevelEditorPlugin.Entities.{entityName}") != null)
            {
                types.Remove(type);
                return;
            }

            string code = CreateSpatialReferenceEntityBoilerplate(type);
            var attr = type.GetCustomAttribute<FrostySdk.Attributes.EbxClassMetaAttribute>();
            string newPath = outPath + $"/Reference/{attr.Namespace}/{entityName}.cs";

            FileInfo fi = new FileInfo(newPath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            using (TextWriter writer = new StreamWriter(new FileStream(newPath, FileMode.Create)))
            {
                writer.WriteLine(code);
            }

            types.Remove(type);
        }

        private void RecursivelyCreateShapeBoilerplate(Type type, List<Type> types, string outPath)
        {
            string entityName = CreateEntityName(type);
            if (type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialEntityData)) || type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialReferenceObjectData)))
            {
                Type tmpType = type;
                while (tmpType != typeof(FrostySdk.Ebx.EntityData))
                {
                    types.Remove(tmpType);
                    tmpType = tmpType.BaseType;
                }

                return;
            }

            if (type.BaseType != typeof(FrostySdk.Ebx.BaseShapeData))
            {
                RecursivelyCreateShapeBoilerplate(type.BaseType, types, outPath);
            }

            if (Type.GetType($"LevelEditorPlugin.Entities.{entityName}") != null)
            {
                types.Remove(type);
                return;
            }

            string code = CreateShapeBoilerplate(type);
            var attr = type.GetCustomAttribute<FrostySdk.Attributes.EbxClassMetaAttribute>();
            string newPath = outPath + $"/Shapes/{attr.Namespace}/{entityName}.cs";

            FileInfo fi = new FileInfo(newPath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            using (TextWriter writer = new StreamWriter(new FileStream(newPath, FileMode.Create)))
            {
                writer.WriteLine(code);
            }

            types.Remove(type);
        }

        private void RecursivelyCreateComponentBoilerplate(Type type, List<Type> types, string outPath)
        {
            string entityName = CreateEntityName(type);
            if (type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialEntityData)) || type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialReferenceObjectData)))
            {
                Type tmpType = type;
                while (tmpType != typeof(FrostySdk.Ebx.EntityData))
                {
                    types.Remove(tmpType);
                    tmpType = tmpType.BaseType;
                }

                return;
            }

            if (type.BaseType != typeof(FrostySdk.Ebx.ComponentData))
            {
                RecursivelyCreateComponentBoilerplate(type.BaseType, types, outPath);
            }

            if (Type.GetType($"LevelEditorPlugin.Entities.{entityName}") != null)
            {
                types.Remove(type);
                return;
            }

            string code = CreateComponentBoilerplate(type);
            var attr = type.GetCustomAttribute<FrostySdk.Attributes.EbxClassMetaAttribute>();
            string newPath = outPath + $"/Components/{attr.Namespace}/{entityName}.cs";

            FileInfo fi = new FileInfo(newPath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            using (TextWriter writer = new StreamWriter(new FileStream(newPath, FileMode.Create)))
            {
                writer.WriteLine(code);
            }

            types.Remove(type);
        }

        private void RecursivelyCreateTimelineTrackBoilerplate(Type type, List<Type> types, string outPath)
        {
            string entityName = CreateEntityName(type);
            if (type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialEntityData)) || type.IsSubclassOf(typeof(FrostySdk.Ebx.SpatialReferenceObjectData)))
            {
                Type tmpType = type;
                while (tmpType != typeof(FrostySdk.Ebx.EntityData))
                {
                    types.Remove(tmpType);
                    tmpType = tmpType.BaseType;
                }

                return;
            }

            if (type.BaseType != typeof(FrostySdk.Ebx.TimelineTrackData))
            {
                RecursivelyCreateTimelineTrackBoilerplate(type.BaseType, types, outPath);
            }

            if (Type.GetType($"LevelEditorPlugin.Entities.{entityName}") != null)
            {
                types.Remove(type);
                return;
            }

            string code = CreateTimelineTrackBoilerplate(type);
            var attr = type.GetCustomAttribute<FrostySdk.Attributes.EbxClassMetaAttribute>();
            string newPath = outPath + $"/Tracks/{attr.Namespace}/{entityName}.cs";

            FileInfo fi = new FileInfo(newPath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            using (TextWriter writer = new StreamWriter(new FileStream(newPath, FileMode.Create)))
            {
                writer.WriteLine(code);
            }

            types.Remove(type);
        }

        private string CreateEntityBoilerplate(Type entityType)
        {
            StringBuilder sb = new StringBuilder();

            string entityName = CreateEntityName(entityType);
            string parentEntity = (entityType.BaseType != typeof(FrostySdk.Ebx.EntityData)) ? CreateEntityName(entityType.BaseType) : "LogicEntity";

            PropertyInfo pi = entityType.GetProperty("Realm");

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            sb.AppendLine("namespace LevelEditorPlugin.Entities");
            sb.AppendLine("{");

            sb.AppendLine($"\t[EntityBinding(DataType = typeof({entityType.FullName}))]");
            sb.AppendLine($"\tpublic class {entityName} : {parentEntity}, IEntityData<{entityType.FullName}>");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tpublic new {entityType.FullName} Data => data as {entityType.FullName};");
            sb.AppendLine($"\t\tpublic override string DisplayName => \"{CreateDisplayName(entityType.Name)}\";");
            if (pi != null && pi.PropertyType == typeof(FrostySdk.Ebx.Realm))
            {
                sb.AppendLine($"\t\tpublic override FrostySdk.Ebx.Realm Realm => Data.Realm;");
            }
            sb.AppendLine();
            sb.AppendLine($"\t\tpublic {entityName}({entityType.FullName} inData, Entity inParent)");
            sb.AppendLine("\t\t\t: base(inData, inParent)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string CreateSpatialEntityBoilerplate(Type entityType)
        {
            StringBuilder sb = new StringBuilder();

            string entityName = CreateEntityName(entityType);
            string parentEntity = (entityType.BaseType != typeof(FrostySdk.Ebx.SpatialEntityData)) ? CreateEntityName(entityType.BaseType) : "SpatialEntity";

            PropertyInfo pi = entityType.GetProperty("Realm");

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            sb.AppendLine("namespace LevelEditorPlugin.Entities");
            sb.AppendLine("{");

            sb.AppendLine($"\t[EntityBinding(DataType = typeof({entityType.FullName}))]");
            sb.AppendLine($"\tpublic class {entityName} : {parentEntity}, IEntityData<{entityType.FullName}>");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tpublic new {entityType.FullName} Data => data as {entityType.FullName};");
            if (pi != null && pi.PropertyType == typeof(FrostySdk.Ebx.Realm))
            {
                sb.AppendLine($"\t\tpublic override FrostySdk.Ebx.Realm Realm => Data.Realm;");
            }
            sb.AppendLine();
            sb.AppendLine($"\t\tpublic {entityName}({entityType.FullName} inData, Entity inParent)");
            sb.AppendLine("\t\t\t: base(inData, inParent)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string CreateSpatialReferenceEntityBoilerplate(Type entityType)
        {
            StringBuilder sb = new StringBuilder();

            string entityName = CreateEntityName(entityType);
            string parentEntity = (entityType.BaseType != typeof(FrostySdk.Ebx.ReferenceObjectData)) ? CreateEntityName(entityType.BaseType) : "ReferenceObject";

            PropertyInfo pi = entityType.GetProperty("Realm");

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            sb.AppendLine("namespace LevelEditorPlugin.Entities");
            sb.AppendLine("{");

            sb.AppendLine($"\t[EntityBinding(DataType = typeof({entityType.FullName}))]");
            sb.AppendLine($"\tpublic class {entityName} : {parentEntity}, IEntityData<{entityType.FullName}>");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tpublic new {entityType.FullName} Data => data as {entityType.FullName};");
            if (pi != null && pi.PropertyType == typeof(FrostySdk.Ebx.Realm))
            {
                sb.AppendLine($"\t\tpublic override FrostySdk.Ebx.Realm Realm => Data.Realm;");
            }
            sb.AppendLine();
            sb.AppendLine($"\t\tpublic {entityName}({entityType.FullName} inData, Entity inParent)");
            sb.AppendLine("\t\t\t: base(inData, inParent)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string CreateShapeBoilerplate(Type shapeType)
        {
            StringBuilder sb = new StringBuilder();

            string entityName = CreateEntityName(shapeType);
            string parentEntity = (shapeType.BaseType != typeof(FrostySdk.Ebx.BaseShapeData)) ? CreateEntityName(shapeType.BaseType) : "BaseShape";

            sb.AppendLine("using Frosty.Core.Viewport;");
            sb.AppendLine("using SharpDX;");
            sb.AppendLine();

            sb.AppendLine("namespace LevelEditorPlugin.Entities");
            sb.AppendLine("{");

            sb.AppendLine($"\t[EntityBinding(DataType = typeof({shapeType.FullName}))]");
            sb.AppendLine($"\tpublic class {entityName} : {parentEntity}, IEntityData<{shapeType.FullName}>");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tpublic new {shapeType.FullName} Data => data as {shapeType.FullName};");
            sb.AppendLine($"\t\tpublic override string DisplayName => \"{CreateDisplayName(shapeType.Name)}\";");
            sb.AppendLine();
            sb.AppendLine($"\t\tpublic {entityName}({shapeType.FullName} inData, Entity inParent)");
            sb.AppendLine("\t\t\t: base(inData, inParent)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t}");
            if (parentEntity == "ShapeEntity")
            {
                sb.AppendLine();
                sb.AppendLine($"\t\tpublic override Matrix GetLocalTransform()");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\treturn Matrix.Identity; // @todo: Implement actual object transform");
                sb.AppendLine("\t\t}");
                sb.AppendLine();
                sb.AppendLine($"\t\tpublic override Matrix GetTransform()");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\treturn Matrix.Identity; // @todo: Implement actual object transform");
                sb.AppendLine("\t\t}");
                sb.AppendLine();
                sb.AppendLine($"\t\tpublic override void SetTransform(Matrix m, bool suppressUpdate)");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\t//@todo");
                sb.AppendLine("\t\t}");
            }
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string CreateComponentBoilerplate(Type componentType)
        {
            StringBuilder sb = new StringBuilder();

            string entityName = CreateEntityName(componentType);
            string parentEntity = (componentType.BaseType != typeof(FrostySdk.Ebx.ComponentData)) ? CreateEntityName(componentType.BaseType) : "Component";

            sb.AppendLine();

            sb.AppendLine("namespace LevelEditorPlugin.Entities");
            sb.AppendLine("{");

            sb.AppendLine($"\t[EntityBinding(DataType = typeof({componentType.FullName}))]");
            sb.AppendLine($"\tpublic class {entityName} : {parentEntity}, IEntityData<{componentType.FullName}>");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tpublic new {componentType.FullName} Data => data as {componentType.FullName};");
            sb.AppendLine($"\t\tpublic override string DisplayName => \"{CreateDisplayName(componentType.Name)}\";");
            sb.AppendLine();
            sb.AppendLine($"\t\tpublic {entityName}({componentType.FullName} inData, Entity inParent)");
            sb.AppendLine("\t\t\t: base(inData, inParent)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string CreateTimelineTrackBoilerplate(Type componentType)
        {
            StringBuilder sb = new StringBuilder();

            string entityName = CreateEntityName(componentType);
            string parentEntity = (componentType.BaseType != typeof(FrostySdk.Ebx.TimelineTrackData)) ? CreateEntityName(componentType.BaseType) : "TimelineTrack";

            sb.AppendLine();

            sb.AppendLine("namespace LevelEditorPlugin.Entities");
            sb.AppendLine("{");

            sb.AppendLine($"\t[EntityBinding(DataType = typeof({componentType.FullName}))]");
            sb.AppendLine($"\tpublic class {entityName} : {parentEntity}, IEntityData<{componentType.FullName}>");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tpublic new {componentType.FullName} Data => data as {componentType.FullName};");
            sb.AppendLine($"\t\tpublic override string DisplayName => \"{CreateDisplayName(componentType.Name)}\";");
            sb.AppendLine();
            sb.AppendLine($"\t\tpublic {entityName}({componentType.FullName} inData, Entity inParent)");
            sb.AppendLine("\t\t\t: base(inData, inParent)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string CreateEntityName(Type type)
        {
            string entityName = type.Name;
            if (entityName.EndsWith("Data")) entityName = entityName.Remove(entityName.Length - 4);
            return entityName;
        }

        private string CreateDisplayName(string name)
        {
            if (name.EndsWith("EntityData")) name = name.Remove(name.Length - 10);
            else if (name.EndsWith("ObjectData")) name = name.Remove(name.Length - 10);
            else if (name.EndsWith("Data")) name = name.Remove(name.Length - 4);
            return name;
        }
    }
}
