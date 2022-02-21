using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace Frosty.Core.Controls
{
    public class FrostyOpenFileDialog
    {
        public string FileName => ofd.FileName;
        public string[] FileNames => ofd.FileNames;
        public int FilterIndex => ofd.FilterIndex;

        public bool Multiselect { get => ofd.Multiselect; set => ofd.Multiselect = value; }
        public string Title { get => ofd.Title; set => ofd.Title = value; }

        private string key;
        private OpenFileDialog ofd;

        public FrostyOpenFileDialog(string title, string filter, string inKey)
        {
            key = inKey + "ImportPath";
            DirectoryInfo di = new DirectoryInfo(Config.Get(key, new FileInfo(Assembly.GetExecutingAssembly().FullName).DirectoryName));
            //DirectoryInfo di = new DirectoryInfo(Config.Get("DialogPaths", key, new FileInfo(Assembly.GetExecutingAssembly().FullName).DirectoryName));

            ofd = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = false,
                InitialDirectory = di.Exists ? di.FullName : ""
            };
        }

        public bool ShowDialog()
        {
            if (ofd.ShowDialog() == true)
            {
                Config.Add(key, Path.GetDirectoryName(ofd.FileName));
                //Config.Add("DialogPaths", key, Path.GetDirectoryName(ofd.FileName));
                Config.Save();
                return true;
            }

            return false;
        }
    }

    public class FrostySaveFileDialog
    {
        public string FileName => sfd.FileName;
        public string InitialDirectory { get => sfd.InitialDirectory; set => sfd.InitialDirectory = value; }
        public int FilterIndex => sfd.FilterIndex;

        private string key;
        private SaveFileDialog sfd;

        public FrostySaveFileDialog(string title, string filter, string inKey, string filename = "", bool overwritePrompt = true)
        {
            key = inKey + "ExportPath";
            DirectoryInfo di = new DirectoryInfo(Config.Get(key, new FileInfo(Assembly.GetExecutingAssembly().FullName).DirectoryName));
            //DirectoryInfo di = new DirectoryInfo(Config.Get("DialogPaths", key, new FileInfo(Assembly.GetExecutingAssembly().FullName).DirectoryName));

            sfd = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
                FileName = filename,
                OverwritePrompt = overwritePrompt,
                InitialDirectory = di.Exists ? di.FullName : ""
            };
        }

        public bool ShowDialog()
        {
            if (sfd.ShowDialog() == true)
            {
                Config.Add(key, Path.GetDirectoryName(sfd.FileName));
                //Config.Add("DialogPaths", key, Path.GetDirectoryName(sfd.FileName));
                Config.Save();
                return true;
            }

            return false;
        }
    }
}
