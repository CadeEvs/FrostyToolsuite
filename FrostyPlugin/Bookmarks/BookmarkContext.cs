using System;
using System.Collections.Generic;

namespace Frosty.Core.Bookmarks
{
    /// <summary>
    /// A BookmarkContext describes a list of bookmarks, the <see cref="BookmarkTarget"/> that would be created 
    /// if the user were to request a bookmark to be created, and events that are raised whenever the available target becomes available or unavailable.
    /// </summary>
    public class BookmarkContext
    {
        /// <summary>
        /// The name of the BookmarkContext, as shown in the contexts LisBox and in the <see cref="BookmarkDb"/>.
        /// </summary>
        public string Name { get; }

        private BookmarkTarget availableTarget;
        /// <summary>
        /// Gets or sets the <see cref="BookmarkTarget"/> currently being made available for bookmarking from this context.
        /// </summary>
        public BookmarkTarget AvailableTarget
        {
            get => availableTarget;
            set
            {
                availableTarget = value;
                if (value != null)
                {
                    TargetAvailable?.Invoke(this, new EventArgs());
                }
                else
                {
                    TargetUnavailable?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Raised after <see cref="AvailableTarget"/> has been set to a new non-null value.
        /// </summary>
        public event EventHandler TargetAvailable;

        /// <summary>
        /// Raised after <see cref="AvailableTarget"/> has been set to null.
        /// </summary>
        public event EventHandler TargetUnavailable;

        /// <summary>
        /// The list of <see cref="BookmarkTarget"/>s that have been bookmarked for this context.
        /// </summary>
        public List<BookmarkItem> Bookmarks { get; set; } = new List<BookmarkItem>();

        /// <summary>
        /// Constructs a new BookmarkContext with the given name and initial bookmarks.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bookmarks"></param>
        public BookmarkContext(string name, List<BookmarkItem> bookmarks = null)
        {
            Name = name;
            if (bookmarks != null)
                Bookmarks = bookmarks;
        }
    }
}
