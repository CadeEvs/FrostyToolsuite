
namespace Frosty.Core.Mod
{
    public sealed class ModSettings
    {
        public bool IsDirty => isDirty;
        public string Title
        {
            get => title;
            set
            {
                if (!title.Equals(value))
                {
                    title = value;
                    isDirty = true;
                }
            }
        }
        public string Author
        {
            get => author;
            set
            {
                if (!author.Equals(value))
                {
                    author = value;
                    isDirty = true;
                }
            }
        }
        public string Category
        {
            get => category;
            set
            {
                if (!category.Equals(value))
                {
                    category = value;
                    isDirty = true;
                }
            }
        }
        public int SelectedCategory
        {
            get => selectedCategory;
            set
            {
                if (!selectedCategory.Equals(value))
                {
                    selectedCategory = value;
                    isDirty = true;
                }
            }
        }
        public string Version
        {
            get => version;
            set
            {
                if (!version.Equals(value))
                {
                    version = value;
                    isDirty = true;
                }
            }
        }
        public string Description
        {
            get => description;
            set
            {
                if (!description.Equals(value))
                {
                    description = value;
                    isDirty = true;
                }
            }
        }
        public string Link
        {
            get => link;
            set
            {
                if (!link.Equals(value))
                {
                    link = value;
                    isDirty = true;
                }
            }
        }
        public byte[] Icon
        {
            get => iconData;
            set
            {
                iconData = value;
                isDirty = true;
            }
        }

        private string title = "";
        private string author = "";
        private string category = "";
        private int selectedCategory = 0;
        private string version = "";
        private string description = "";
        private string link = "";

        private byte[] iconData = null;
        private byte[][] screenshotData = null;

        private bool isDirty;

        public ModSettings()
        {
            screenshotData = new byte[4][];
        }

        public void SetScreenshot(int index, byte[] buffer) { screenshotData[index] = buffer; isDirty = true; }
        public byte[] GetScreenshot(int index) { return screenshotData[index]; }
        public void ClearDirtyFlag() { isDirty = false; }
    }
}
