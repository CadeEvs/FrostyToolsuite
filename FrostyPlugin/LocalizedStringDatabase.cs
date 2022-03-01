using System.Collections.Generic;

namespace Frosty.Core
{
    /// <summary>
    /// Defines actions that can be performed on a localized string database. Can be obtained using <see cref="LocalizedStringDatabase.Current"/>
    /// </summary>
    public interface ILocalizedStringDatabase
    {
        /// <summary>
        /// Initializes the localized string database.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Retrieves the string with the specified ID from the localized string database.
        /// </summary>
        /// <param name="id">The ID of the string to obtain.</param>
        /// <returns>The string with the specified ID from the localized string database.</returns>
        string GetString(uint id);

        /// <summary>
        /// Retrieves the string with the specified string ID (Typically begins with ID_) from the localized string database.
        /// </summary>
        /// <param name="stringId">The string ID of the string to obtain.</param>
        /// <returns>The string with the specified string ID from the localized string database.</returns>
        string GetString(string stringId);

        /// <summary>
        /// Sets the value of the string with the specified ID from the localized string database.
        /// </summary>
        /// <param name="id">The ID of the string to obtain.</param>
        void SetString(uint id, string value);

        /// <summary>
        /// Sets the value of the string with the specified ID from the localized string database.
        /// </summary>
        /// <param name="id">The ID of the string to obtain.</param>
        void SetString(string id, string value);

        /// <summary>
        /// Reverts the value of the string with the specified ID from the localized string database.
        /// </summary>
        /// <param name="id">The ID of the string to obtain.</param>
        void RevertString(uint id);

        /// <summary>
        /// Checks if the string with the specified ID from the localized string database is modified.
        /// </summary>
        /// <param name="id">The ID of the string to obtain.</param>
        bool isStringEdited(uint id);

        /// <summary>
        /// Opens a window to add strings to the localized string database.
        /// </summary>
        void AddStringWindow();

        /// <summary>
        /// Opens a window to replace all occurances of a string in the localized string database.
        /// </summary>
        void BulkReplaceWindow();

        /// <summary>
        /// Retrieves a collection of string IDs that currently reside in the localized string database.
        /// </summary>
        /// <returns>A collection of string IDs, or an empty collection if no strings exist.</returns>
        IEnumerable<uint> EnumerateStrings();

        /// <summary>
        /// Retrieves a collection of string IDs that were modified from the localized string database.
        /// </summary>
        /// <returns>A collection of string IDs, or an empty collection if no modified strings exist.</returns>
        IEnumerable<uint> EnumerateModifiedStrings();
    }

    // represents the default localized string database, in case no plugin with a specialized
    // database is loaded. Will just return blank strings
    internal class DefaultLocalizedStringDatabase : ILocalizedStringDatabase
    {
        public IEnumerable<uint> EnumerateStrings()
        {
            yield break;
        }

        public IEnumerable<uint> EnumerateModifiedStrings()
        {
            yield break;
        }

        public string GetString(uint id)
        {
            return "";
        }

        public string GetString(string stringId)
        {
            return "";
        }

        public void SetString(uint id, string value)
        {
            return;
        }

        public void SetString(string id, string value)
        {
            return;
        }

        public void RevertString(uint id)
        {
            return;
        }

        public void AddStringWindow()
        {
            return;
        }

        public void BulkReplaceWindow()
        {
            return;
        }

        public bool isStringEdited(uint id)
        {
            return false;
        }

        public void Initialize()
        {
        }
    }

    /// <summary>
    /// Provides access to the currently loaded localized string database.
    /// </summary>
    public sealed class LocalizedStringDatabase
    {
        /// <summary>
        /// Gets the currently loaded localized string database.
        /// </summary>
        /// <returns>The currently loaded localized string database.</returns>
        public static ILocalizedStringDatabase Current { get; internal set; }
    }
}
