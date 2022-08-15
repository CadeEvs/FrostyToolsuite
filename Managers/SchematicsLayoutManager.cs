using Frosty.Core;
using FrostySdk;
using FrostySdk.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FrostySdk.Managers;

namespace LevelEditorPlugin.Managers
{
    public struct SchematicsLayout
    {
        public struct InterfaceData
        {
            public int NameHash;
            public int FieldType;
            public int Direction;
        }

        public struct ShortcutData
        {
            public List<Shortcut> Shortcuts;
        }

        public struct Shortcut
        {
            public Guid UniqueId;
            public string DisplayName;
            public Point Position;
            public InterfaceData? ExtraData;
            public List<Point> ChildPositions;
            public List<Guid> ChildGuids;
        }

        public struct WirePoint
        {
            public Guid UniqueId;
            public Point Position;
        }

        public struct Node
        {
            public Guid UniqueId;
            public Guid FileGuid;
            public Guid InstanceGuid;
            public Point Position;
            public bool IsCollapsed;
            public InterfaceData? InterfaceData;
            public ShortcutData? ShortcutData;
        }

        public struct Wire
        {
            public int Index;
            public List<WirePoint> WirePoints;
        }

        public struct Comment
        {
            public Guid UniqueId;
            public string Text;
            public Color Color;
            public Point Position;
            public List<Guid> Children;
        }

        public string BlueprintName;
        public Guid FileGuid;
        public Point CanvasPosition;
        public double CanvasScale;
        public List<Node> Nodes;
        public List<Wire> Wires;
        public List<Comment> Comments;
    }

    public class SchematicsLayoutManager
    {
        #region -- Singleton --

        public static SchematicsLayoutManager Instance { get; private set; } = new SchematicsLayoutManager();
        private SchematicsLayoutManager() { LoadLayouts(); }

        #endregion

        private readonly string m_layoutsPath = $"{App.ProfileSettingsPath}/layouts";
        private readonly string m_blankProjectName = "New Project.fbproject";

        private Dictionary<Guid, SchematicsLayout> m_layouts;
        private Dictionary<Guid, SchematicsLayout> m_projectLayouts;
        private string m_currentProject;

        public void LoadProjectLayouts(string project)
        {
            if (m_currentProject != project)
            {
                if (project == m_blankProjectName)
                {
                    m_currentProject = null;
                    m_projectLayouts.Clear();
                }
                else
                {
                    m_currentProject = project;
                    string layoutsFilename = $"{project}.layouts.json";

                    if (File.Exists(layoutsFilename))
                    {
                        using (StreamReader streamReader = new StreamReader(layoutsFilename))
                        {
                            m_projectLayouts = JsonConvert.DeserializeObject<Dictionary<Guid, SchematicsLayout>>(streamReader.ReadToEnd());
                        }
                    }
                    else
                    {
                        m_projectLayouts = new Dictionary<Guid, SchematicsLayout>();
                    }
                }
            }
        }

        public SchematicsLayout? GetLayout(Guid guid)
        {
            if (!string.IsNullOrEmpty(m_currentProject))
            {
                if (m_projectLayouts.ContainsKey(guid))
                    return m_projectLayouts[guid];
            }

            if (!m_layouts.ContainsKey(guid))
                return null;

            return m_layouts[guid];
        }

        public void AddLayout(Guid guid, SchematicsLayout layout)
        {
            if (!string.IsNullOrEmpty(m_currentProject))
            {
                if (!m_projectLayouts.ContainsKey(guid))
                {
                    m_projectLayouts.Add(guid, layout);
                }
                else
                {
                    m_projectLayouts[guid] = layout;
                }
            }
            else
            {
                if (!m_layouts.ContainsKey(guid))
                {
                    m_layouts.Add(guid, layout);
                }
                else
                {
                    m_layouts[guid] = layout;
                }
            }

            SaveLayouts();
        }

        public void ClearLayout(Guid guid)
        {
            if (!string.IsNullOrEmpty(m_currentProject))
            {
                if (m_layouts.ContainsKey(guid))
                {
                    m_layouts.Remove(guid);
                    SaveLayouts();
                    return;
                }
            }

            if (!m_layouts.ContainsKey(guid))
                return;

            m_layouts.Remove(guid);
            SaveLayouts();
        }

        public void PrintLayouts(ILogger logger)
        {
            foreach (Guid guid in m_layouts.Keys)
            {
                EbxAssetEntry entry = App.AssetManager.GetEbxEntry(guid);
                logger.Log(entry.Name);
            }
        }

        private void LoadLayouts()
        { 
            m_layouts = new Dictionary<Guid, SchematicsLayout>();

            // load layouts if folder exists
            if (Directory.Exists(m_layoutsPath))
            {
                foreach (string filePath in Directory.GetFiles(m_layoutsPath, ".", SearchOption.AllDirectories))
                {
                    using (StreamReader streamReader = new StreamReader(filePath))
                    {
                        KeyValuePair<Guid, SchematicsLayout> layout = JsonConvert.DeserializeObject<KeyValuePair<Guid, SchematicsLayout>>(streamReader.ReadToEnd());
                        m_layouts.Add(layout.Key, layout.Value);
                    }
                }
            }
        }

        private void SaveLayouts()
        {
            // project
            if (!string.IsNullOrEmpty(m_currentProject))
            {
                string layoutsFilename = $"{m_currentProject}.layouts.json";
                using (StreamWriter streamWriter = new StreamWriter(layoutsFilename))
                {
                    streamWriter.WriteLine(JsonConvert.SerializeObject(m_projectLayouts, Formatting.Indented));
                }
            }

            // global
            foreach (KeyValuePair<Guid, SchematicsLayout> layout in m_layouts)
            {
                string fileName = $"{m_layoutsPath}/{layout.Value.BlueprintName}.json";
                FileInfo fi = new FileInfo(fileName);
                if (!fi.Directory.Exists)
                {
                    Directory.CreateDirectory(fi.DirectoryName);
                }
                
                using (StreamWriter streamWriter = new StreamWriter(fileName))
                {
                    streamWriter.WriteLine(JsonConvert.SerializeObject(layout, Formatting.Indented));
                }
            }
        }
    }
}
