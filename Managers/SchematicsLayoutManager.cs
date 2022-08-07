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

        private readonly string LayoutsFilename = $"{App.ProfileSettingsPath}/layouts.json";
        private readonly string BlankProjectName = "New Project.fbproject";

        private Dictionary<Guid, SchematicsLayout> layouts;
        private Dictionary<Guid, SchematicsLayout> projectLayouts;
        private string currentProject;

        public void LoadProjectLayouts(string project)
        {
            if (currentProject != project)
            {
                if (project == BlankProjectName)
                {
                    currentProject = null;
                    projectLayouts.Clear();
                }
                else
                {
                    currentProject = project;
                    string layoutsFilename = $"{project}.layouts.json";

                    if (File.Exists(layoutsFilename))
                    {
                        using (StreamReader streamReader = new StreamReader(layoutsFilename))
                        {
                            projectLayouts = JsonConvert.DeserializeObject<Dictionary<Guid, SchematicsLayout>>(streamReader.ReadToEnd());
                        }
                    }
                    else
                    {
                        projectLayouts = new Dictionary<Guid, SchematicsLayout>();
                    }
                }
            }
        }

        public SchematicsLayout? GetLayout(Guid guid)
        {
            if (!string.IsNullOrEmpty(currentProject))
            {
                if (projectLayouts.ContainsKey(guid))
                    return projectLayouts[guid];
            }

            if (!layouts.ContainsKey(guid))
                return null;

            return layouts[guid];
        }

        public void AddLayout(Guid guid, SchematicsLayout layout)
        {
            if (!string.IsNullOrEmpty(currentProject))
            {
                if (!projectLayouts.ContainsKey(guid))
                {
                    projectLayouts.Add(guid, layout);
                }
                else
                {
                    projectLayouts[guid] = layout;
                }
            }
            else
            {
                if (!layouts.ContainsKey(guid))
                {
                    layouts.Add(guid, layout);
                }
                else
                {
                    layouts[guid] = layout;
                }
            }

            SaveLayouts();
        }

        public void ClearLayout(Guid guid)
        {
            if (!string.IsNullOrEmpty(currentProject))
            {
                if (layouts.ContainsKey(guid))
                {
                    layouts.Remove(guid);
                    SaveLayouts();
                    return;
                }
            }

            if (!layouts.ContainsKey(guid))
                return;

            layouts.Remove(guid);
            SaveLayouts();
        }

        public void PrintLayouts(ILogger logger)
        {
            foreach (Guid guid in layouts.Keys)
            {
                EbxAssetEntry entry = App.AssetManager.GetEbxEntry(guid);
                logger.Log(entry.Name);
            }
        }

        private void LoadLayouts()
        {
            if (File.Exists(LayoutsFilename))
            {
                using (StreamReader streamReader = new StreamReader(LayoutsFilename))
                {
                    layouts = JsonConvert.DeserializeObject<Dictionary<Guid, SchematicsLayout>>(streamReader.ReadToEnd());
                }
            }
            else
            {
                layouts = new Dictionary<Guid, SchematicsLayout>();
            }
        }

        private void SaveLayouts()
        {
            if (!string.IsNullOrEmpty(currentProject))
            {
                string layoutsFilename = $"{currentProject}.layouts.json";
                using (StreamWriter streamWriter = new StreamWriter(layoutsFilename))
                {
                    streamWriter.WriteLine(JsonConvert.SerializeObject(projectLayouts, Formatting.Indented));
                }
            }

            FileInfo fi = new FileInfo(LayoutsFilename);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
            
            using (StreamWriter streamWriter = new StreamWriter(LayoutsFilename))
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(layouts, Formatting.Indented));
            }
        }
    }
}
