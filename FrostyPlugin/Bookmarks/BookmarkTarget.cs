namespace Frosty.Core.Bookmarks
{
    /// <summary>
    /// The base class for destinations of links in Frosty.
    /// </summary>
    public abstract class BookmarkTarget
    {
        /// <summary>
        /// A short but unique textual representation of the target.
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// An icon that represents the type of target.
        /// </summary>
        public abstract System.Windows.Media.ImageSource Icon { get; }

        /// <summary>
        /// Navigates to the target.
        /// </summary>
        public abstract void NavigateTo(bool bOpen);

        /// <summary>
        /// Returns a <see cref="string"/> that can be used to store this bookmark for later loading via <see cref="LoadData(string)"/>.
        /// </summary>
        public string Serialize => GetType().FullName + ":" + SerializeData();

        /// <summary>
        /// Returns a <see cref="string"/> that can be used to recreate this target.
        /// </summary>
        /// <returns></returns>
        protected abstract string SerializeData();

        /// <summary>
        /// Restores the state of a target that has been stored in the given string.
        /// </summary>
        /// <param name="serializedData"></param>
        public abstract void LoadData(string serializedData);

        /// <summary>
        /// Determines whether this target is identical to another bookmark target by checking the types and the serialized state.
        /// </summary>
        /// <param name="obj">The other target to check against.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BookmarkTarget other) || !obj.GetType().Equals(GetType()))
                return false;

            return other.Serialize == Serialize;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
