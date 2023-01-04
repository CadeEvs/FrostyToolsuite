using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Frosty.Core.Attributes;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using Frosty.Core.Mod;
using FrostySdk.Managers.Entries;

namespace Frosty.Core
{
    /// <summary>
    /// Describes when the plugin should be loaded.
    /// </summary>
    public enum PluginLoadType
    {
        /// <summary>
        /// The plugin loads on app startup.
        /// </summary>
        Startup,
        /// <summary>
        /// The plugin loads during the profile loading process.
        /// </summary>
        Initialize
    }

    /// <summary>
    /// Describes the context in which the plugin manager is loading.
    /// </summary>
    public enum PluginManagerType
    {
        /// <summary>
        /// The plugin is loading into the editor.
        /// </summary>
        Editor,

        /// <summary>
        /// The plugin is loading into the mod manager.
        /// </summary>
        ModManager,

        /// <summary>
        /// The plugin is loading for both the editor and mod manager.
        /// </summary>
        Both
    }

    /// <summary>
    /// Represents a shader loaded from a plugin
    /// </summary>
    public sealed class ShaderDefinition
    {
        /// <summary>
        /// Gets the plugin in which the shader is located.
        /// </summary>
        /// <returns>The <see cref="Assembly"/> that represents the plugin.</returns>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets the name of the shader resource file in the format of Namespace.ResourceName.
        /// </summary>
        /// <returns>The name of the shader resource file.</returns>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the type of shader.
        /// </summary>
        /// <returns>The type of shader.</returns>
        public ShaderType ShaderType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderDefinition"/> class with the plugin, shader type, and resource name.
        /// </summary>
        /// <param name="plugin">The plugin in which the shader is located.</param>
        /// <param name="type">The type of shader.</param>
        /// <param name="resourceName">The resource name of the shader in the format of Namespace.ResourceName.</param>
        public ShaderDefinition(Assembly plugin, ShaderType type, string resourceName)
        {
            Assembly = plugin;
            ShaderType = type;
            ResourceName = resourceName;
        }
    }

    public class Plugin
    {
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Version { get; private set; }

        public Assembly Assembly { get; private set; }

        public Plugin(string name, string author, string version, Assembly assembly)
        {
            Name = name;
            Author = author;
            Version = version;

            Assembly = assembly;
        }
    }

    /// <summary>
    /// Represents the core plugin manager responsible for loading and managing of plugin extensions.
    /// </summary>
    public sealed class PluginManager
    {
        /// <summary>
        /// Retreives a collection of menu extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of menu extensions.</returns>
        public IEnumerable<MenuExtension> MenuExtensions => m_menuExtensions;

        /// <summary>
        /// Retreives a collection of toolbar extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of toolbar extensions.</returns>
        public IEnumerable<ToolbarExtension> ToolbarExtensions => m_toolbarExtensions;
        
        /// <summary>
        /// Retreives a collection of menu extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of menu extensions.</returns>
        public IEnumerable<TabExtension> TabExtensions => m_tabExtensions;

        /// <summary>
        /// Retreives a collection of data explorer context menu item extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of menu extensions.</returns>
        public IEnumerable<DataExplorerContextMenuExtension> DataExplorerContextMenuExtensions => m_contextMenuItemExtensions;

        /// <summary>
        /// Retrieves a collection of options extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of options extensions.</returns>
        public IEnumerable<Type> OptionsExtensions => m_optionsExtensions;

        /// <summary>
        /// Retreives a collection of profiles that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of profiles.</returns>
        public IEnumerable<Profile> Profiles => m_profiles;

        /// <summary>
        /// Retreives a collection of startup actions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of startup actions.</returns>
        public IEnumerable<StartupAction> StartupActions => m_startupActions;

        /// <summary>
        /// Retreives a collection of execution actions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of execution actions.</returns>
        public IEnumerable<ExecutionAction> ExecutionActions => m_executionActions;

        /// <summary>
        /// Retreives a collection of loaded plugins.
        /// </summary>
        /// <returns>A collection of loaded plugins.</returns>
        public IEnumerable<Plugin> LoadedPlugins => m_loadedPlugins;

        /// <summary>
        /// Retreives an enum defining which manager is being loaded right now.
        /// </summary>
        /// <returns>An enum defining which manager is being loaded right now..</returns>
        public PluginManagerType ManagerType => m_managerType;

        public IEnumerable<string> CustomAssetHandlers => m_customAssetHandlers.Keys;

        private readonly Dictionary<string, AssetDefinition> m_definitions = new Dictionary<string, AssetDefinition>();
        private readonly List<MenuExtension> m_menuExtensions = new List<MenuExtension>();
        private readonly List<ToolbarExtension> m_toolbarExtensions = new List<ToolbarExtension>();
        private readonly List<TabExtension> m_tabExtensions = new List<TabExtension>();
        private readonly List<DataExplorerContextMenuExtension> m_contextMenuItemExtensions = new List<DataExplorerContextMenuExtension>();
        private readonly Dictionary<string, Type> m_globalTypEditors = new Dictionary<string, Type>();
        private readonly Dictionary<string, Type> m_typeOverrides = new Dictionary<string, Type>();
        private readonly List<Type> m_optionsExtensions = new List<Type>();
        private readonly List<string> m_thirdPartyDlls = new List<string>();
        private readonly List<Profile> m_profiles = new List<Profile>();
        private readonly List<Plugin> m_plugins = new List<Plugin>();
        private readonly List<Plugin> m_loadedPlugins = new List<Plugin>();
        private readonly List<ExecutionAction> m_executionActions = new List<ExecutionAction>();
        private readonly List<StartupAction> m_startupActions = new List<StartupAction>();
        private readonly Dictionary<uint, Type> m_customHandlers = new Dictionary<uint, Type>();
        private readonly Dictionary<string, Type> m_customAssetHandlers = new Dictionary<string, Type>();
        private readonly Dictionary<ResourceType, Type> m_resCustomHandlers = new Dictionary<ResourceType, Type>();
        private readonly Dictionary<string, ShaderDefinition[]> m_shaders = new Dictionary<string, ShaderDefinition[]>();
        private readonly List<string> m_userShaders = new List<string>();
        
        private Type m_localizedStringDatabaseType;
        private readonly PluginManagerType m_managerType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class with the specified logger and context.
        /// </summary>
        /// <param name="logger">The logging interface the plugin manager should use.</param>
        /// <param name="context">The context which this plugin should load for.</param>
        public PluginManager(ILogger logger, PluginManagerType context)
        {
            m_managerType = context;

            logger.Log("");
            logger.Log($"Scanning '{new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName}\\Plugins' for plugins");

            // load from main executable
            LoadDefinitionsFromAssembly(PluginLoadType.Startup, Assembly.GetEntryAssembly());
            LoadDefinitionsFromAssembly(PluginLoadType.Startup, Assembly.GetExecutingAssembly());
            int pluginCount = 0;

            // now load from plugins directory
            if (Directory.Exists("Plugins"))
            {
                foreach (string pluginPath in Directory.EnumerateFiles("Plugins", "*.dll", SearchOption.AllDirectories))
                {
                    FileInfo fi = new FileInfo(pluginPath);
                    string statusText = "Located plugin ";

                    try
                    {
                        var assembly = Assembly.LoadFile(fi.FullName);

                        LoadDefinitionsFromAssembly(PluginLoadType.Startup, assembly);

                        var displayName = assembly.GetCustomAttribute<PluginDisplayNameAttribute>()?.DisplayName;
                        var author = assembly.GetCustomAttribute<PluginAuthorAttribute>()?.Author;
                        var version = assembly.GetCustomAttribute<PluginVersionAttribute>()?.Version;

                        if (string.IsNullOrEmpty(displayName))
                            displayName = fi.Name;
                        if (string.IsNullOrEmpty(author))
                            author = "Unknown";
                        if (string.IsNullOrEmpty(version))
                            version = "1.0.0.0";

                        statusText += displayName + " (v" + version + ") by " + author;
                        pluginCount++;

                        m_plugins.Add(new Plugin(displayName, author, version, assembly));
                        logger.Log(statusText);
                    }
                    catch (Exception e)
                    {
                        statusText += fi.Name + " - Failed (" + e.Message + ")";
                        logger.Log(statusText);
                    }
                }
            }

            logger.Log("Found a total of {0} plugins", pluginCount);
            logger.Log("");
        }

        /// <summary>
        /// Initializes all of the post-profile assembly plugins.
        /// </summary>
        public void Initialize()
        {
            foreach (Plugin plugin in m_plugins)
            {
                if (!IsValidToLoadPlugin(plugin.Assembly))
                {
                    continue;
                }

                LoadDefinitionsFromAssembly(PluginLoadType.Initialize, plugin.Assembly);
                m_loadedPlugins.Add(plugin);
            }
        }

        /// <summary>
        /// Clears out every registered plugin extension
        /// </summary>
        public void Clear()
        {
            m_definitions.Clear();
            
            m_menuExtensions.Clear();
            m_toolbarExtensions.Clear();
            m_tabExtensions.Clear();
            m_contextMenuItemExtensions.Clear();
            //globalTypEditors.Clear();
            //typeOverrides.Clear();
            m_optionsExtensions.Clear();
            m_thirdPartyDlls.Clear();
            //profiles.Clear();
            m_loadedPlugins.Clear();
            m_executionActions.Clear();
            //startupActions.Clear();
            m_customHandlers.Clear();
            m_resCustomHandlers.Clear();
            m_customAssetHandlers.Clear();
            m_shaders.Clear();
            m_userShaders.Clear();
        }

        /// <summary>
        /// Returns the <see cref="AssetDefinition"/> with the specified type.
        /// </summary>
        /// <param name="type">The type of the asset definition to obtain.</param>
        /// <returns>The <see cref="AssetDefinition"/>.</returns>
        public AssetDefinition GetAssetDefinition(string type)
        {
            if (type == null)
            {
                return null;
            }

            type = type.ToLower();
            return !m_definitions.ContainsKey(type) ? null : m_definitions[type];
        }

        /// <summary>
        /// Returns the <see cref="Type"/> to use to construct the type editor with the specified lookup name.
        /// </summary>
        /// <param name="lookupName">The lookup name of the type editor to obtain.</param>
        /// <returns>The <see cref="Type"/> of the type editor.</returns>
        public Type GetTypeEditor(string lookupName)
        {
            lookupName = lookupName.ToLower();
            if (!m_globalTypEditors.ContainsKey(lookupName))
            {
                return null;
            }
            return m_globalTypEditors[lookupName];
        }

        /// <summary>
        /// Returns the <see cref="Type"/> of the type override class with the specified lookup name.
        /// </summary>
        /// <param name="lookupName">The lookup name of the type override to obtain.</param>
        /// <returns>The <see cref="Type"/> of the type override.</returns>
        public Type GetTypeOverride(string lookupName)
        {
            lookupName = lookupName.ToLower();
            if (!m_typeOverrides.ContainsKey(lookupName))
            {
                return null;
            }
            return m_typeOverrides[lookupName];
        }

        /// <summary>
        /// Returns true if a plugin has registered the specified name as a third party DLL.
        /// </summary>
        /// <param name="name">The name of the DLL to check.</param>
        /// <returns>True if a plugin has registered the specified name as a third party DLL.</returns>
        public bool IsThirdPartyDll(string name)
        {
            name = name.ToLower();
            return m_thirdPartyDlls.Contains(name);
        }

        /// <summary>
        /// Returns true if the loaded manager is the same a the specific manager type.
        /// </summary>
        /// <returns>A boolean if the loaded manager is the specified manager type.</returns>
        public bool IsManagerType(PluginManagerType type) => type == m_managerType;

        /// <summary>
        /// Returns the <see cref="Assembly"/> of the specified plugin if it is loaded. Returns null otherwise.
        /// </summary>
        /// <param name="name">The name of the plugin to obtain.</param>
        /// <returns>The <see cref="Assembly"/> of the plugin.</returns>
        public Assembly GetPluginAssembly(string name)
        {
            foreach (var plugin in m_plugins)
            {
                if (plugin.Assembly.GetName().Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return plugin.Assembly;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates and returns the localized string database. Or a default one if one was not specified by any
        /// of the loaded plugin.
        /// </summary>
        /// <returns>The constructed <see cref="ILocalizedStringDatabase"/>.</returns>
        public ILocalizedStringDatabase GetLocalizedStringDatabase()
        {
            ILocalizedStringDatabase localizedStringDb;
            if (m_localizedStringDatabaseType == null)
            {
                localizedStringDb = new DefaultLocalizedStringDatabase();
            }
            else
            {
                localizedStringDb = (ILocalizedStringDatabase)Activator.CreateInstance(m_localizedStringDatabaseType);
            }

            LocalizedStringDatabase.Current = localizedStringDb;
            return localizedStringDb;
        }

        public ICustomActionHandler GetCustomHandler(string ebxType)
        {
            uint hash = (uint)Fnv1.HashString(ebxType.ToLower());
            return GetCustomHandler(hash);
        }

        public ICustomActionHandler GetCustomHandler(uint handlerHash)
        {
            if (!m_customHandlers.ContainsKey(handlerHash))
            {
                return null;
            }
            return (ICustomActionHandler)Activator.CreateInstance(m_customHandlers[handlerHash]);
        }

        public ICustomActionHandler GetCustomHandler(ResourceType resType)
        {
            if (!m_resCustomHandlers.ContainsKey(resType))
            {
                return null;
            }
            return (ICustomActionHandler)Activator.CreateInstance(m_resCustomHandlers[resType]);
        }

        public ICustomAssetCustomActionHandler GetCustomAssetHandler(string type)
        {
            if (!m_customAssetHandlers.ContainsKey(type))
            {
                return null;
            }
            return (ICustomAssetCustomActionHandler)Activator.CreateInstance(m_customAssetHandlers[type]);
        }

        /// <summary>
        /// Returns the shader bytecode of the shader specified by the name and shader type.
        /// </summary>
        /// <param name="type">The <see cref="ShaderType"/> of the shader.</param>
        /// <param name="name">The resource name of the shader.</param>
        /// <returns>The bytecode of the shader.</returns>
        public byte[] GetShader(ShaderType type, string name)
        {
            if (!m_shaders.ContainsKey(name))
            {
                return null;
            }

            ShaderDefinition shaderDef = null;
            switch (type)
            {
                case ShaderType.VertexShader: shaderDef = m_shaders[name][0]; break;
                case ShaderType.ComputeShader: shaderDef = m_shaders[name][0]; break;
                case ShaderType.PixelShader: shaderDef = m_shaders[name][1]; break;
            }

            if (shaderDef == null)
            {
                return null;
            }

            return NativeReader.ReadInStream(shaderDef.Assembly.GetManifestResourceStream(shaderDef.ResourceName));
        }

        public IEnumerable<string> GetUserShaders()
        {
            return m_userShaders;
        }

        // returns true if the specified plugin should be loaded (Based on ValidFor and NotValidFor attributes)
        private bool IsValidToLoadPlugin(Assembly assembly)
        {
            List<PluginValidForProfileAttribute> validAttrs = new List<PluginValidForProfileAttribute>();
            List<PluginNotValidForProfileAttribute> notValidAttrs = new List<PluginNotValidForProfileAttribute>();

            validAttrs.AddRange(assembly.GetCustomAttributes<PluginValidForProfileAttribute>());
            notValidAttrs.AddRange(assembly.GetCustomAttributes<PluginNotValidForProfileAttribute>());

            foreach (var attr in notValidAttrs)
            {
                if (ProfilesLibrary.DataVersion == attr.ProfileVersion)
                {
                    return false;
                }
            }

            bool retVal = false;
            if (validAttrs.Count == 0)
            {
                retVal = true;
            }
            else
            {
                foreach (var attr in validAttrs)
                {
                    if (ProfilesLibrary.DataVersion == attr.ProfileVersion)
                    {
                        retVal = true;
                        break;
                    }
                }
            }

            return retVal;
        }

        // loads all the necessary definitions from a plugin based on the exported attributes.
        private void LoadDefinitionsFromAssembly(PluginLoadType loadType, Assembly assembly)
        {
            foreach (Attribute tmpAttr in assembly.GetCustomAttributes())
            {
                if (m_managerType == PluginManagerType.ModManager
                    && !(tmpAttr is RegisterCustomHandlerAttribute)
                    && !(tmpAttr is RegisterExecutionAction)
                    && !(tmpAttr is RegisterOptionsExtensionAttribute))
                {
                    continue;
                }

                if (loadType == PluginLoadType.Startup)
                {
                    if (tmpAttr is RegisterProfileAttribute attr1)
                    {
                        m_profiles.Add(((IProfile)Activator.CreateInstance(attr1.ProfileType)).CreateProfile());
                    }
                    else if (tmpAttr is RegisterTypeOverrideAttribute attr4)
                    {
                        m_typeOverrides.Add(attr4.LookupName.ToLower(), attr4.EditorType);
                    }
                    else if (tmpAttr is RegisterGlobalTypeEditorAttribute attr3)
                    {
                        m_globalTypEditors.Add(attr3.LookupName.ToLower(), attr3.EditorType);
                    }
                    else if (tmpAttr is RegisterStartupActionAttribute attr2)
                    {
                        m_startupActions.Add((StartupAction)Activator.CreateInstance(attr2.StartupActionType));
                    }
                }
                else if (loadType == PluginLoadType.Initialize)
                {
                    if (tmpAttr is RegisterAssetDefinitionAttribute attr1)
                    {
                        Type assetType = TypeLibrary.GetType(attr1.LookupType);
                        if (assetType != null)
                        {
                            AssetDefinition definition = (AssetDefinition)Activator.CreateInstance(attr1.AssetDefinitionType);
                            foreach (Type subType in TypeLibrary.GetDerivedTypes(assetType))
                            {
                                string subName = subType.Name.ToLower();
                                if (!m_definitions.ContainsKey(subName))
                                {
                                    m_definitions.Add(subName, definition);
                                }
                            }

                            string name = assetType.Name.ToLower();
                            if (!m_definitions.ContainsKey(name))
                            {
                                m_definitions.Add(name, definition);
                            }
                        }
                        else
                        {
                            // most likely a legacy file type
                            AssetDefinition definition = (AssetDefinition)Activator.CreateInstance(attr1.AssetDefinitionType);

                            string name = attr1.LookupType.ToLower();
                            if (!m_definitions.ContainsKey(name))
                            {
                                m_definitions.Add(name, definition);
                            }
                        }
                    }
                    else if (tmpAttr is RegisterMenuExtensionAttribute attr2)
                    {
                        if (!attr2.MenuExtensionType.IsSubclassOf(typeof(MenuExtension)))
                            throw new Exception("Menu extensions must extend from MenuExtensions base class");
                        m_menuExtensions.Add((MenuExtension)Activator.CreateInstance(attr2.MenuExtensionType));
                    }
                    else if (tmpAttr is RegisterToolbarExtensionAttribute attr3)
                    {
                        if (!attr3.ToolbarExtensionType.IsSubclassOf(typeof(ToolbarExtension)))
                            throw new Exception("Toolbar extensions must extend from MenuExtensions base class");
                        m_toolbarExtensions.Add((ToolbarExtension)Activator.CreateInstance(attr3.ToolbarExtensionType));
                    }
                    else if (tmpAttr is RegisterOptionsExtensionAttribute attr5)
                    {
                        if (attr5.ManagerType == m_managerType || attr5.ManagerType == PluginManagerType.Both)
                        {
                            if (!attr5.OptionsType.IsSubclassOf(typeof(OptionsExtension)))
                                throw new Exception("Option extensions must extend from OptionsExtension base class");
                            m_optionsExtensions.Add(attr5.OptionsType);
                        }
                    }
                    else if (tmpAttr is RegisterThirdPartyDllAttribute attr6)
                    {
                        string dllname = attr6.DllName.ToLower();
                        if (!m_thirdPartyDlls.Contains(dllname))
                            m_thirdPartyDlls.Add(dllname);
                    }
                    else if (tmpAttr is RegisterLocalizedStringDatabaseAttribute attr7)
                    {
                        if (m_localizedStringDatabaseType == null && attr7.LocalizedStringDatabaseType.GetInterface("ILocalizedStringDatabase") != null)
                            m_localizedStringDatabaseType = attr7.LocalizedStringDatabaseType;
                    }
                    else if (tmpAttr is RegisterShaderAttribute attr8)
                    {
                        if (!m_shaders.ContainsKey(attr8.KeyName))
                            m_shaders.Add(attr8.KeyName, new ShaderDefinition[2]);

                        if ((attr8.ShaderType == ShaderType.VertexShader || attr8.ShaderType == ShaderType.ComputeShader) && m_shaders[attr8.KeyName][0] == null)
                            m_shaders[attr8.KeyName][0] = new ShaderDefinition(assembly, attr8.ShaderType, attr8.ResourceName);
                        if (attr8.ShaderType == ShaderType.PixelShader && m_shaders[attr8.KeyName][1] == null)
                            m_shaders[attr8.KeyName][1] = new ShaderDefinition(assembly, attr8.ShaderType, attr8.ResourceName);
                    }
                    else if (tmpAttr is RegisterTabExtensionAttribute attr9)
                    {
                        if (!attr9.TabExtensionType.IsSubclassOf(typeof(TabExtension)))
                            throw new Exception("Tab extensions must extend from MenuExtensions base class");
                        m_tabExtensions.Add((TabExtension)Activator.CreateInstance(attr9.TabExtensionType));
                    }
                    else if (tmpAttr is RegisterDataExplorerContextMenuAttribute attr10)
                    {
                        if (!attr10.ContextMenuItemExtensionType.IsSubclassOf(typeof(DataExplorerContextMenuExtension)))
                            throw new Exception("Data Explorer context menu item extensions must extend from ContextMenuExtensions base class");
                        m_contextMenuItemExtensions.Add((DataExplorerContextMenuExtension)Activator.CreateInstance(attr10.ContextMenuItemExtensionType));
                    }
                    else if (tmpAttr is RegisterExecutionAction attr11)
                    {
                        m_executionActions.Add((ExecutionAction)Activator.CreateInstance(attr11.ExecutionActionType));
                    }
                    else if (tmpAttr is RegisterCustomHandlerAttribute attr)
                    {
                        if (attr.HandlerType == CustomHandlerType.Ebx)
                        {
                            var assetType = TypeLibrary.GetType(attr.EbxType);
                            if (assetType != null)
                            {
                                foreach (var subType in TypeLibrary.GetDerivedTypes(assetType))
                                {
                                    if (!m_customHandlers.ContainsKey((uint)Fnv1.HashString(subType.Name.ToLower())))
                                    {
                                        m_customHandlers.Add((uint)Fnv1.HashString(subType.Name.ToLower()), attr.HandlerClassType);
                                    }
                                }
                                if (!m_customHandlers.ContainsKey((uint)Fnv1.HashString(assetType.Name.ToLower())))
                                {
                                    m_customHandlers.Add((uint)Fnv1.HashString(assetType.Name.ToLower()), attr.HandlerClassType);
                                }
                            }
                        }
                        else if (attr.HandlerType == CustomHandlerType.Res)
                        {
                            if (!m_resCustomHandlers.ContainsKey(attr.ResType))
                            {
                                m_resCustomHandlers.Add(attr.ResType, attr.HandlerClassType);
                            }
                        }
                        else if (attr.HandlerType == CustomHandlerType.CustomAsset)
                        {
                            m_customAssetHandlers.Add(attr.CustomAssetType, attr.HandlerClassType);
                        }
                    }
                    else if (tmpAttr is RegisterUserShaderAttribute registerUserShaderAttribute)
                    {
                        if (registerUserShaderAttribute != null)
                        {
                            m_userShaders.Add(registerUserShaderAttribute.XmlDescriptor + "," + registerUserShaderAttribute.ShaderName);
                        }
                    }
                    else if (tmpAttr is RegisterCustomAssetManagerAttribute attr12)
                    {
                        App.AssetManager.RegisterCustomAssetManager(attr12.CustomAssetManagerType, attr12.CustomAssetManagerClassType);
                    }
                }
            }
        }
    }
}
