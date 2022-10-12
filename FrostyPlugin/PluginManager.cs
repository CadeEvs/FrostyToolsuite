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

namespace Frosty.Core
{
    /// <summary>
    /// Represents the core plugin manager responsible for loading and managing of plugin extensions.
    /// </summary>
    public sealed class PluginManager
    {
        private List<DataExplorerContextMenuExtension> m_contextMenuItemExtensions = new List<DataExplorerContextMenuExtension>();

        private Dictionary<uint, Type> m_customHandlers = new Dictionary<uint, Type>();

        private Dictionary<string, AssetDefinition> m_definitions = new Dictionary<string, AssetDefinition>();

        private List<ExecutionAction> m_executionActions = new List<ExecutionAction>();

        private Dictionary<string, Type> m_globalTypeEditors = new Dictionary<string, Type>();

        private List<Plugin> m_loadedPlugins = new List<Plugin>();

        private Type m_localizedStringDatabaseType;

        private PluginManagerType m_managerType;

        private List<MenuExtension> m_menuExtensions = new List<MenuExtension>();

        private List<Type> m_optionsExtensions = new List<Type>();

        private List<Plugin> m_plugins = new List<Plugin>();

        private List<Profile> m_profiles = new List<Profile>();

        private Dictionary<ResourceType, Type> m_resCustomHandlers = new Dictionary<ResourceType, Type>();

        private Dictionary<string, ShaderDefinition[]> m_shaders = new Dictionary<string, ShaderDefinition[]>();

        private List<StartupAction> m_startupActions = new List<StartupAction>();

        private List<TabExtension> m_tabExtensions = new List<TabExtension>();

        private List<string> m_thirdPartyDlls = new List<string>();

        private Dictionary<string, Type> m_typeOverrides = new Dictionary<string, Type>();

        private List<string> m_userShaders = new List<string>();

        /// <summary>
        /// Retrieves a collection of data explorer context menu item extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of menu extensions.</returns>
        public IEnumerable<DataExplorerContextMenuExtension> DataExplorerContextMenuExtensions => m_contextMenuItemExtensions;

        /// <summary>
        /// Retrieves a collection of execution actions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of execution actions.</returns>
        public IEnumerable<ExecutionAction> ExecutionActions => m_executionActions;

        /// <summary>
        /// Retrieves a collection of loaded plugins.
        /// </summary>
        /// <returns>A collection of loaded plugins.</returns>
        public IEnumerable<Plugin> LoadedPlugins => m_loadedPlugins;

        /// <summary>
        /// Retrieves an enum defining which manager is being loaded right now.
        /// </summary>
        /// <returns>An enum defining which manager is being loaded right now..</returns>
        public PluginManagerType ManagerType => m_managerType;

        /// <summary>
        /// Retrieves a collection of menu extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of menu extensions.</returns>
        public IEnumerable<MenuExtension> MenuExtensions => m_menuExtensions;

        /// <summary>
        /// Retrieves a collection of options extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of options extensions.</returns>
        public IEnumerable<Type> OptionsExtensions => m_optionsExtensions;

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> of all plugins that have been found.
        /// </summary>
        public IEnumerable<Plugin> Plugins => m_plugins;

        /// <summary>
        /// Retrieves a collection of profiles that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of profiles.</returns>
        public IEnumerable<Profile> Profiles => m_profiles;

        /// <summary>
        /// Retrieves a collection of menu extensions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of menu extensions.</returns>
        public IEnumerable<TabExtension> TabExtensions => m_tabExtensions;

        /// <summary>
        /// Retrieves a collection of startup actions that have been loaded from plugins.
        /// </summary>
        /// <returns>A collection of startup actions.</returns>
        public IEnumerable<StartupAction> StartupActions => m_startupActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class with the specified logger and context.
        /// </summary>
        /// <param name="logger">The logging interface the plugin manager should use.</param>
        /// <param name="context">The context which this plugin should load for.</param>
        public PluginManager(ILogger logger, PluginManagerType context)
        {
            m_managerType = context;

            // Check if the plugins directory doesn't exist
            if (!Directory.Exists("Plugins"))
            {
                // Inform the user of the missing directory
                logger.Log("The \"Plugins\" directory could not be located within the executable's associated directory. Due to some plugins being a necessity for functionality, please retrieve a copy of the needed directory from a clean archive of the editor/mod manager.");

                // Prevent further execution
                return;
            }

            LoadDefinitionsFromAssembly(PluginLoadType.Startup, Assembly.GetEntryAssembly());
            LoadDefinitionsFromAssembly(PluginLoadType.Startup, Assembly.GetExecutingAssembly());

            foreach (string item in Directory.EnumerateFiles("Plugins", "*.dll", SearchOption.AllDirectories))
            {
                FileInfo fileInfo = new FileInfo(item);

                // Add the plugin to the list of located plugins
                m_plugins.Add(LoadPlugin(fileInfo.FullName, PluginLoadType.Startup));
            }
        }

        /// <summary>
        /// Initializes all of the post-profile assembly plugins.
        /// </summary>
        public void Initialize()
        {
            foreach (Plugin plugin in m_plugins)
            {
                if (IsValidToLoadPlugin(plugin.Assembly))
                {
                    LoadDefinitionsFromAssembly(PluginLoadType.Initialize, plugin.Assembly);

                    // since IsValidToLoadPlugin does not block out Startup plugins, we can simply use this method as a means of adding to the loadedPlugins list
                    // this also filters out plugins invalid for the current profile
                    m_loadedPlugins.Add(plugin);
                    continue;
                }

                // assign to the current plugin's status with an invalid load status if it did not pass the validity checks
                plugin.Status = PluginLoadStatus.LoadedInvalid;
            }
        }

        /// <summary>
        /// Returns the <see cref="AssetDefinition"/> with the specified type.
        /// </summary>
        /// <param name="type">The type of the asset definition to obtain.</param>
        /// <returns>The <see cref="AssetDefinition"/>.</returns>
        public AssetDefinition GetAssetDefinition(string type)
        {
            if (type == null)
                return null;

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
            if (!m_globalTypeEditors.ContainsKey(lookupName))
                return null;
            return m_globalTypeEditors[lookupName];
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
                return null;
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
                // if the plugin does not have an assigned assembly, skip it
                if (plugin.Assembly?.GetName().Name.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)
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
                localizedStringDb = new DefaultLocalizedStringDatabase();
            else
                localizedStringDb = (ILocalizedStringDatabase)Activator.CreateInstance(m_localizedStringDatabaseType);

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
                return null;
            return (ICustomActionHandler)Activator.CreateInstance(m_customHandlers[handlerHash]);
        }

        public ICustomActionHandler GetCustomHandler(ResourceType resType)
        {
            if (!m_resCustomHandlers.ContainsKey(resType))
                return null;
            return (ICustomActionHandler)Activator.CreateInstance(m_resCustomHandlers[resType]);
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
                return null;

            ShaderDefinition shaderDef = null;
            switch (type)
            {
                case ShaderType.VertexShader: shaderDef = m_shaders[name][0]; break;
                case ShaderType.ComputeShader: shaderDef = m_shaders[name][0]; break;
                case ShaderType.PixelShader: shaderDef = m_shaders[name][1]; break;
            }

            if (shaderDef == null)
                return null;

            return NativeReader.ReadInStream(shaderDef.Assembly.GetManifestResourceStream(shaderDef.ResourceName));
        }

        public IEnumerable<string> GetUserShaders()
        {
            return m_userShaders;
        }

        /// <summary>
        /// Attempts to load a plugin located at the specified <paramref name="pluginPath"/>.
        /// </summary>
        /// <param name="pluginPath">The path to the target plugin.</param>
        /// <param name="loadType">Optional. The <see cref="PluginLoadType"/> that determines what should be loaded from the plugin.</param>
        /// <returns>A <see cref="Plugin"/> instance regardless of whether or not a failure to load the plugin occurred. You may check for the potential of such failures through the <see cref="Plugin.Status"/> property.</returns>
        public Plugin LoadPlugin(string pluginPath, PluginLoadType? loadType = null)
        {
            Plugin loadedPlugin = new Plugin(null, pluginPath);

            // create a variable of type Assembly for temporarily storing the loaded assembly until LoadDefinitionsFromAssembly finishes executing, allowing us to tell whether or not the assembly is a valid plugin before storing it with a Plugin instance
            Assembly pluginAssembly;

            // utilize a try catch statement to collect plugin load errors
            try
            {
                pluginAssembly = Assembly.LoadFile(loadedPlugin.SourcePath);

                switch (loadType)
                {
                    case null:
                        // attempt to load everything from the plugin
                        LoadDefinitionsFromAssembly(PluginLoadType.Startup, pluginAssembly);

                        // since initialization plugins may be game-specific, ensure the plugin may be loaded for the active profile before proceeding
                        if (IsValidToLoadPlugin(pluginAssembly))
                        {
                            LoadDefinitionsFromAssembly(PluginLoadType.Initialize, pluginAssembly);

                            loadedPlugin.Status = PluginLoadStatus.Loaded;
                            break;
                        }

                        loadedPlugin.Status = PluginLoadStatus.LoadedInvalid;
                        break;

                    case PluginLoadType.Startup:
                    case PluginLoadType.Initialize:
                        LoadDefinitionsFromAssembly(loadType.Value, pluginAssembly);

                        loadedPlugin.Status = PluginLoadStatus.Loaded;
                        break;
                }

                // the assembly may now be stored within the plugin instance's Assembly property
                loadedPlugin.Assembly = pluginAssembly;
            }
            catch (Exception e)
            {
                loadedPlugin.LoadException = e;
            }

            return loadedPlugin;
        }

        /// <summary>
        /// Determines whether or not a plugin is valid for the active profile.
        /// </summary>
        /// <param name="assembly">The plugin assembly to be checked.</param>
        /// <returns>A bool determining whether or not the given plugin is valid for the active profile.</returns>
        private bool IsValidToLoadPlugin(Assembly assembly)
        {
            if (assembly == null)
            {
                // return true to have the plugin "loaded," as if we were to return false, the plugin would be marked invalid
                return true;
            }

            List<PluginValidForProfileAttribute> validAttrs = new List<PluginValidForProfileAttribute>();
            List<PluginNotValidForProfileAttribute> notValidAttrs = new List<PluginNotValidForProfileAttribute>();

            validAttrs.AddRange(assembly.GetCustomAttributes<PluginValidForProfileAttribute>());
            notValidAttrs.AddRange(assembly.GetCustomAttributes<PluginNotValidForProfileAttribute>());

            foreach (var attr in notValidAttrs)
            {
                if (ProfilesLibrary.IsLoaded((ProfileVersion)attr.ProfileVersion))
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
                    if (ProfilesLibrary.IsLoaded((ProfileVersion)attr.ProfileVersion))
                    {
                        retVal = true;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Loads all plugin definitions of a specified <see cref="PluginLoadType"/> from a given assembly.
        /// </summary>
        /// <param name="loadType">The type of plugins to be located and loaded.</param>
        /// <param name="assembly">The assembly to be checked for plugin definitions.</param>
        private void LoadDefinitionsFromAssembly(PluginLoadType loadType, Assembly assembly)
        {
            if (assembly == null)
            {
                return;
            }

            foreach (var tmpAttr in assembly.GetCustomAttributes())
            {
                if (m_managerType == PluginManagerType.ModManager && !(tmpAttr is RegisterCustomHandlerAttribute) && !(tmpAttr is RegisterExecutionAction) && !(tmpAttr is RegisterOptionsExtensionAttribute))
                    continue;

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
                        m_globalTypeEditors.Add(attr3.LookupName.ToLower(), attr3.EditorType);
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
                            foreach (var subType in TypeLibrary.GetDerivedTypes(assetType))
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
                                    m_customHandlers.Add((uint)Fnv1.HashString(subType.Name.ToLower()), attr.HandlerClassType);
                                }
                                m_customHandlers.Add((uint)Fnv1.HashString(assetType.Name.ToLower()), attr.HandlerClassType);
                            }
                        }
                        else if (attr.HandlerType == CustomHandlerType.Res)
                        {
                            m_resCustomHandlers.Add(attr.ResType, attr.HandlerClassType);
                        }
                    }
                    else if (tmpAttr is RegisterUserShaderAttribute registerUserShaderAttribute)
                    {
                        if (registerUserShaderAttribute != null)
                        {
                            m_userShaders.Add(registerUserShaderAttribute.XmlDescriptor + "," + registerUserShaderAttribute.ShaderName);
                            continue;
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
