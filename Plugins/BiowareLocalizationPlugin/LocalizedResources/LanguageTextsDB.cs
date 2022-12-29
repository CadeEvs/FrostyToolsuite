using Frosty.Core;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BiowareLocalizationPlugin.LocalizedResources
{

    /// <summary>
    /// Most of the actual get/set and find Texts methods per language are located in here. This class then forwards calls to the correct LocalizedStringResource instances.
    /// </summary>
    public class LanguageTextsDB
    {

        #region -- HelperClass
        /// <summary>
        /// Stores information about which resources a text is located in in the vanilla game, and which resources where added by mods.
        /// </summary>
        private class TextLocation
        {
            /// <summary>
            /// The id of the specific text.
            /// </summary>
            public uint TextId { get; }

            /// <summary>
            /// The resource names with occurences of that text in the vanilla game.
            /// </summary>
            public ISet<string> DefaultResourceNames { get; }

            /// <summary>
            /// The resource names where a mod added the text to.
            /// </summary>
            public ISet<string> AddedResourceNames = new HashSet<string>();

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="inTextId"></param>
            public TextLocation(uint inTextId)
            {
                TextId = inTextId;
                DefaultResourceNames = new HashSet<string>();
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="inTextId"></param>
            /// <param name="defaultResources"></param>
            public TextLocation(uint inTextId, string inDefaultResource)
            {
                TextId = inTextId;
                DefaultResourceNames = new HashSet<string>() { inDefaultResource };
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="inTextId"></param>
            /// <param name="inResourceName"></param>
            /// /// <param name="inIsAddedResource">true if the resourcename belongs to a modified resource, false if it is amodified resource</param>
            public TextLocation(uint inTextId, string inResourceName, bool inIsAddedResource)
            {
                TextId = inTextId;
                DefaultResourceNames = new HashSet<string>();

                if (inIsAddedResource)
                {
                    AddedResourceNames.Add(inResourceName);
                }
                else
                {
                    DefaultResourceNames.Add(inResourceName);
                }
            }

            /// <summary>
            /// Adds the given resourcename to the list of added resources, if it is not already present.
            /// </summary>
            /// <param name="resourceName"></param>
            public void AddLocation(string resourceName)
            {
                if (!DefaultResourceNames.Contains(resourceName))
                {
                    AddedResourceNames.Add(resourceName);
                }
            }
        }

        // Enum specifying which resource location to retrieve for a text id.
        private enum ResourceLocationRequest { DEFAULT_ONLY, ADDED, ALL };
        #endregion

        /// <summary>
        /// The name of the language format in the game.
        /// </summary>
        public string LanguageIdentifier { get; private set; }

        /// <summary>
        /// This dictionary contains all the found text ids as well as their text.
        /// </summary>
        private readonly Dictionary<uint, string> m_textsForId = new Dictionary<uint, string>();

        /// <summary>
        /// This dictionary contains all resource assets where a string was found.
        /// </summary>
        private readonly Dictionary<uint, TextLocation> m_resourcesForStringId = new Dictionary<uint, TextLocation>();

        /// <summary>
        /// The resources available
        /// </summary>
        private readonly SortedDictionary<string, LocalizedStringResource> m_resourcesByName = new SortedDictionary<string, LocalizedStringResource>();

        /// <summary>
        /// Lists the names of all resources that include declinated adjectives (both of them for dai, one for mea).
        /// </summary>
        private readonly SortedSet<string> m_declinatedAdjectiveIncludingResurces = new SortedSet<string>();

        public void Init(string languageName, IEnumerable<string> bundlePaths)
        {
            LanguageIdentifier = languageName;
            LoadTextResources(bundlePaths);
        }

        /// <summary>
        /// Tries to return the text for the given uid. Returns an error message if the text does not exist.
        /// @see #FindText
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetText(uint id)
        {
            if (!m_textsForId.ContainsKey(id))
            {
                return id == 0 ? "" : string.Format("Invalid StringId: {0}", id.ToString("X8"));
            }
            return m_textsForId[id];
        }

        /// <summary>
        /// Tries to return the text for the given uid, returns null if the textid does not exist.
        /// @see #GetString
        /// </summary>
        /// <param name="textId"></param>
        /// <returns></returns>
        public string FindText(uint textId)
        {
            bool textExists = m_textsForId.TryGetValue(textId, out string text);
            return textExists ? text : null;
        }

        /// <summary>
        /// Returns the list of LocalizedStringResource in which the given text id can be found by default.
        /// </summary>
        /// <param name="textId">The text id to look for.</param>
        /// <returns>All resources in which the text id can be found by default.</returns>
        public IEnumerable<LocalizedStringResource> GetDefaultResourcesForTextId(uint textId)
        {
            bool exists = m_resourcesForStringId.TryGetValue(textId, out TextLocation textLocation);
            return exists ? GetResources(textLocation, ResourceLocationRequest.DEFAULT_ONLY) : new List<LocalizedStringResource>();
        }

        /// <summary>
        /// Returns the list of LocalizedStringResource in which the given text id can be found.
        /// </summary>
        /// <param name="textId">The text id to look for.</param>
        /// <returns>All resources in which the text id can be found.</returns>
        public IEnumerable<LocalizedStringResource> GetAllResourcesForTextId(uint textId)
        {
            bool exists = m_resourcesForStringId.TryGetValue(textId, out TextLocation textLocation);
            return exists ? GetResources(textLocation, ResourceLocationRequest.ALL) : new List<LocalizedStringResource>();
        }

        /// <summary>
        /// Returns the list of LocalizedStringResource in which the given text id was inserted by a mod.
        /// </summary>
        /// <param name="textId">The text id to look for.</param>
        /// <returns>All non default resources in which the text id can be found.</returns>
        public IEnumerable<LocalizedStringResource> GetAddedResourcesForTextId(uint textId)
        {
            bool exists = m_resourcesForStringId.TryGetValue(textId, out TextLocation textLocation);
            return exists ? GetResources(textLocation, ResourceLocationRequest.ADDED) : new List<LocalizedStringResource>();
        }

        /// <summary>
        /// Returns the names of all found resources
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllResourceNames()
        {
            return m_resourcesByName.Keys;
        }

        /// <summary>
        /// Returns all text ids
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetAllTextIds()
        {
            foreach (uint key in m_textsForId.Keys)
            {
                yield return key;
            }
        }

        public IEnumerable<uint> GetAllModifiedTextsIds()
        {
            HashSet<uint> modifiedTextIds = new HashSet<uint>();

            foreach (LocalizedStringResource resource in m_resourcesByName.Values)
            {
                modifiedTextIds.UnionWith(resource.GetAllModifiedTextsIds());
            }

            return modifiedTextIds;
        }

        /// <summary>
        /// Sets a text into a single resource.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="textId"></param>
        /// <param name="text"></param>
        public void SetText(string resourceName, uint textId, string text)
        {

            LocalizedStringResource resource = GetResourceByName(resourceName);

            resource.SetText(textId, text);

            bool locExists = m_resourcesForStringId.TryGetValue(textId, out TextLocation textLocation);
            if (!locExists)
            {
                textLocation = new TextLocation(textId);
                m_resourcesForStringId.Add(textId, textLocation);
            }
            textLocation.AddLocation(resourceName);
        }

        /// <summary>
        /// Removes a single text from a modified resource.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="textId"></param>
        public void RemoveText(string resourceName, uint textId)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            resource.RemoveText(textId);

            bool textLocationExists = m_resourcesForStringId.TryGetValue(textId, out TextLocation textLocation);
            if (!textLocationExists)
            {
                App.Logger.LogError("TextID <{0}> does not exist for language <{1}>!", textId, LanguageIdentifier);
                return;
            }

            textLocation.AddedResourceNames.Remove(resourceName);

            if (textLocation.AddedResourceNames.Count == 0 && textLocation.DefaultResourceNames.Count == 0)
            {
                m_resourcesForStringId.Remove(textId);
            }
        }

        /// <summary>
        /// Removes a text from the textDB's cache. This usually is only a intermediary operation, until the updated text is reinserted.
        /// </summary>
        /// <param name="textId"></param>
        public void RemoveTextFromCache(uint textId)
        {
            m_textsForId.Remove(textId);
        }

        /// <summary>
        /// Updates the textDb's cache with the new value for the text id.
        /// </summary>
        /// <param name="textId"></param>
        /// <param name="text"></param>
        public void UpdateTextCache(uint textId, string text)
        {
            m_textsForId[textId] = text;
        }

        public void RevertText(uint textId)
        {
            string defaultText = null;
            ISet<string> nonDefaultResourceNames = m_resourcesForStringId[textId].AddedResourceNames;
            var resources = new List<LocalizedStringResource>(GetAllResourcesForTextId(textId));

            foreach (LocalizedStringResource resource in resources)
            {
                if (defaultText == null)
                {
                    defaultText = resource.GetDefaultText(textId);
                }
                resource.RemoveText(textId);
                nonDefaultResourceNames.Remove(resource.Name);
            }

            if (defaultText != null)
            {
                UpdateTextCache(textId, defaultText);
            }
            else
            {
                RemoveTextFromCache(textId);
            }
        }

        public IEnumerable<string> GetAllResourceNamesWithDeclinatedAdjectives()
        {
            return m_declinatedAdjectiveIncludingResurces;
        }

        public IEnumerable<string> GetAllResourceNamesWithModifiedDeclinatedAdjectives()
        {
            foreach (string resourceName in m_declinatedAdjectiveIncludingResurces)
            {
                LocalizedStringResource resource = GetResourceByName(resourceName);

                if (resource.ContainsModifiedDeclinatedAdjectives())
                {
                    yield return resourceName;
                }
            }
        }

        public IEnumerable<uint> GetAllDeclinatedAdjectiveIdsFromResource(string resourceName)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            return resource.GetAllDeclinatedAdjectivesIds();
        }

        public IEnumerable<uint> GetModifiedDeclinatedAdjectiveIdsFromResource(string resourceName)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            return resource.GetAllModifiedDeclinatedAdjectivesIds();
        }

        public List<string> GetDeclinatedAdjectives(string resourceName, uint adjectiveId)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            return resource.GetDeclinatedAdjective(adjectiveId);
        }

        public void SetDeclinatedAdjectve(string resourceName, uint adjectiveId, List<string> aAdjectives)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            resource.SetAdjectiveDeclinations(adjectiveId, aAdjectives);

            m_declinatedAdjectiveIncludingResurces.Add(resourceName);
        }

        public void RevertDeclinatedAdjective(string resourceName, uint adjectiveId)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            resource.RemoveAdjectiveDeclination(adjectiveId);

            if (!resource.ContainsDeclinatedAdjectives())
            {
                m_declinatedAdjectiveIncludingResurces.Remove(resourceName);
            }
        }

        public IEnumerable<uint> GetAllTextIdsFromResource(string resourceName)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            var entries = resource.GetAllPrimaryTexts();
            return entries.Select(tuple => tuple.Item1);
        }

        public IEnumerable<uint> GetAllModifiedTextIdsFromResource(string resourceName)
        {
            LocalizedStringResource resource = GetResourceByName(resourceName);
            return resource.GetAllModifiedTextsIds();
        }

        /// <summary>
        /// Loads the localizedSTringResources from the given budles.
        /// </summary>
        /// <param name="bundlePaths"></param>
        private void LoadTextResources(IEnumerable<string> bundlePaths)
        {

            foreach (string superBundlePathPart in bundlePaths)
            {
                string superBundlePath = "win32/" + superBundlePathPart.ToLowerInvariant();
                foreach (ResAssetEntry resEntry in App.AssetManager.EnumerateRes(resType: (uint)ResourceType.LocalizedStringResource, bundleSubPath: superBundlePath))
                {

                    LocalizedStringResource resource = App.AssetManager.GetResAs<LocalizedStringResource>(resEntry);
                    if (resource != null)
                    {

                        m_resourcesByName[resource.Name] = resource;
                        FetchResourceTexts(resource);

                        resource.ResourceEventHandlers += (s, e) =>
                        {
                            UpdateResourceTexts((LocalizedStringResource)s);
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Gets all texts in the resource and updates the caches.
        /// </summary>
        /// <param name="resource"></param>
        private void FetchResourceTexts(LocalizedStringResource resource)
        {

            string resourceName = resource.Name;
            foreach (var entry in resource.GetAllPrimaryTexts())
            {
                uint textId = entry.Item1;
                bool isModifiedText = entry.Item3;

                m_textsForId[textId] = entry.Item2;

                bool textLocationExists = m_resourcesForStringId.TryGetValue(textId, out TextLocation textlocation);

                if (!textLocationExists)
                {
                    textlocation = new TextLocation(textId, resourceName, isModifiedText);
                    m_resourcesForStringId.Add(textId, textlocation);
                }
                else if (isModifiedText)
                {
                    textlocation.AddLocation(resourceName);
                }
                else
                {
                    textlocation.DefaultResourceNames.Add(resourceName);
                }
            }

            if (resource.ContainsDeclinatedAdjectives())
            {
                m_declinatedAdjectiveIncludingResurces.Add(resourceName);
            }
        }

        private void UpdateResourceTexts(LocalizedStringResource resource)
        {

            // need to remove all texts wiht only resource as location...
            string resourceName = resource.Name;

            // remove via explizit loop, too tired to figure out another way
            List<uint> textIdsToRemove = new List<uint>();
            foreach (TextLocation textloc in m_resourcesForStringId.Values)
            {
                if (textloc.AddedResourceNames.Remove(resourceName) && textloc.DefaultResourceNames.Count == 0)
                {
                    if (textloc.AddedResourceNames.Count == 0)
                    {
                        textIdsToRemove.Add(textloc.TextId);
                    }
                }
            }
            foreach (uint toRemove in textIdsToRemove)
            {
                m_resourcesForStringId.Remove(toRemove);
                m_textsForId.Remove(toRemove);
            }

            FetchResourceTexts(resource);
        }

        /// <summary>
        /// Returns all resources from the given TextLocation that match the given resourceLocation- variant.
        /// </summary>
        /// <param name="textLocation"></param>
        /// <param name="resourceLocations"></param>
        /// <returns></returns>
        private IEnumerable<LocalizedStringResource> GetResources(TextLocation textLocation, ResourceLocationRequest resourceLocations)
        {
            if (ResourceLocationRequest.ALL == resourceLocations || ResourceLocationRequest.DEFAULT_ONLY == resourceLocations)
            {
                foreach (string resourceName in textLocation.DefaultResourceNames)
                {
                    yield return m_resourcesByName[resourceName];
                }
            }

            if (ResourceLocationRequest.ALL == resourceLocations || ResourceLocationRequest.ADDED == resourceLocations)
            {
                foreach (string addedResourceName in textLocation.AddedResourceNames)
                {
                    yield return m_resourcesByName[addedResourceName];
                }
            }
        }

        /// <summary>
        /// Returns the resource of the given name, or throws an exception if the resource does not exist.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private LocalizedStringResource GetResourceByName(string resourceName)
        {
            bool resourceExists = m_resourcesByName.TryGetValue(resourceName, out LocalizedStringResource resource);
            if (!resourceExists)
            {
                throw new InvalidOperationException(string.Format("Resource of name <{0}> does not exist for language <{1}>!", resourceName, LanguageIdentifier));
            }

            return resource;
        }
    }
}
