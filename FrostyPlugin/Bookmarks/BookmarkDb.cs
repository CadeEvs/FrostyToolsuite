using System;
using System.Collections.Generic;

namespace Frosty.Core.Bookmarks
{
    /// <summary>
    /// Stores all the bookmarks and manages the bookmark contexts for the Frosty editor.
    /// </summary>
    public static class BookmarkDb
    {
        /// <summary>
        /// Stores all the registered <see cref="BookmarkContext"/>s and their corresponding names.
        /// </summary>
        public static Dictionary<string, BookmarkContext> Contexts = new Dictionary<string, BookmarkContext>();

        /// <summary>
        /// Raised directly before <see cref="CurrentContext"/> is changed. The new context is the sender argument.
        /// </summary>
        public static event EventHandler ContextChanged;

        private static BookmarkContext currentContext = null;
        /// <summary>
        /// Gets or sets the current <see cref="BookmarkContext"/>
        /// </summary>
        public static BookmarkContext CurrentContext
        {
            get => currentContext;
            set
            {
                ContextChanged?.Invoke(value, new EventArgs());
                currentContext = value;
            }
        }

        /// <summary>
        /// Finds or creates a <see cref="BookmarkContext"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="BookmarkContext"/> to find or create.</param>
        /// <returns></returns>
        public static BookmarkContext GetContext(string name)
        {
            if (Contexts.ContainsKey(name))
            {
                return Contexts[name];
            }

            BookmarkContext newContext = new BookmarkContext(name);
            Contexts.Add(name, newContext);
            if (Contexts.Count == 1)
            {
                CurrentContext = newContext;
            }
            return newContext;
        }

        /// <summary>
        /// Instantiates a <see cref="BookmarkTarget"/> from a serialized bookmark string.
        /// </summary>
        /// <param name="serialized">The serialized bookmark data, in the form 'Namespace.FullTypeName:data'</param>
        /// <returns></returns>
        private static BookmarkTarget SpawnBookmarkTarget(string serialized)
        {
            try
            {
                string[] parts = serialized.Split(':');
                if (parts.Length != 2)
                    throw new ArgumentException("Parameter serialized must contain the BookmarkTarget type and serialized data, separated by a colon.");
                BookmarkTarget obj = (BookmarkTarget)Activator.CreateInstance(Type.GetType(parts[0], true));
                obj.LoadData(parts[1]);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Loads bookmarks and contexts from the Config system.
        /// </summary>
        public static void LoadDb()
        {
            BookmarkContext cxt = null;
            Contexts = new Dictionary<string, BookmarkContext>();
            List<BookmarkItem> allTargets = new List<BookmarkItem>();

            string db = Config.Get("BookmarkDb", "", ConfigScope.Game);
            //string db = Config.Get("Bookmarks", "BookmarkDb", "");
            if (string.IsNullOrEmpty(db))
                return;

            string[] lines = db.Split('|');
            foreach (string line in lines)
            {
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                    continue;
                if (line.StartsWith("["))
                {
                    cxt = new BookmarkContext(line.Substring(1, line.Length - 2));
                    allTargets = new List<BookmarkItem>();
                    Contexts.Add(cxt.Name, cxt);
                    if (Contexts.Count == 1)
                    {
                        CurrentContext = cxt;
                    }
                }
                else
                {
                    BookmarkItem newItem = LoadBookmark(line, allTargets);

                    if (newItem.Parent == null)
                        cxt?.Bookmarks.Add(newItem);
                }
            }
        }

        /// <summary>
        /// Reads a single bookmark and places it in the bookmark tree.
        /// </summary>
        /// <param name="data">The bookmark, serialized as a string.</param>
        /// <param name="allItems">All the bookmarks that have been loaded so far.</param>
        /// <returns></returns>
        private static BookmarkItem LoadBookmark(string data, List<BookmarkItem> allItems)
        {
            string[] parts = data.Split('$');
            BookmarkTarget target = SpawnBookmarkTarget(parts[0]);
            BookmarkItem parent = null;
            string name = "";
            if (parts.Length > 1)
            {
                name = parts[1];
                if (parts.Length > 2)
                    parent = allItems[int.Parse(parts[2])];
            }

            BookmarkItem result = new BookmarkItem(target, parent) {Name = name};
            parent?.Children.Add(result);
            allItems.Add(result);
            return result;
        }

        /// <summary>
        /// Recursively serializes a bookmark hierarchy into a <see cref="List{T}"/>.
        /// </summary>
        /// <param name="item">The root bookmark to serialize.</param>
        /// <param name="count">A running total of how many bookmarks have been serialized so far.</param>
        /// <param name="indexes">A mapping of previously-serialized bookmarks to their indices in the previously-serialized output.</param>
        /// <returns></returns>
        private static List<string> SaveBookmark(BookmarkItem item, ref int count, Dictionary<BookmarkItem, int> indexes)
        {
            List<string> result = new List<string> {item.Target.Serialize + "$" + item.Name + (item.Parent == null ? "" : ("$" + indexes[item.Parent]))};
            indexes.Add(item, count);
            count++;
            foreach (BookmarkItem child in item.Children)
            {
                result.AddRange(SaveBookmark(child, ref count, indexes));
            }
            return result;
        }

        /// <summary>
        /// Writes the entire bookmark database to the Config system.
        /// </summary>
        public static void SaveDb()
        {
            List<string> lines = new List<string>();
            foreach (BookmarkContext cxt in Contexts.Values)
            {
                lines.Add("[" + cxt.Name + "]");
                Dictionary<BookmarkItem, int> indexes = new Dictionary<BookmarkItem, int>();
                int count = 0;
                foreach (BookmarkItem item in cxt.Bookmarks)
                {
                    lines.AddRange(SaveBookmark(item, ref count, indexes));
                }
            }

            Config.Add("BookmarkDb", string.Join("|", lines), ConfigScope.Game);
            Config.Save();
            //Config.Add("Bookmarks", "BookmarkDb", string.Join("|", lines));
        }
    }
}
